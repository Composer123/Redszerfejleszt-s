using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Raktar.DataContext;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Raktar.Services
{
    public interface IUserService
    {
        Task<UserDTO> RegisterAsync(UserRegisterDTO userDto);
        Task<string> LoginAsync(UserLoginDTO userDto);
        Task<UserDTO> UpdateProfileAsync(int userId, UserUpdateDTO userDto);
        Task<UserDTO> UpdateAddressAsync(int userId, SimpleAddressDTO addressDto);
        Task<IList<UserDTO>> GetRolesAsync();
        Task<UserDTO> GetUserByIdAsync(int userId);
    }

    public class UserService : IUserService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(
            WarehouseDbContext context,
            IMapper mapper,
            IConfiguration configuration
        )
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserDTO> RegisterAsync(UserRegisterDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.Password = Encoding.UTF8.GetBytes(PasswordHasher.HashPassword(Encoding.UTF8.GetString(userDto.Password)));
            user.Roles = new List<Role>();

            if (userDto.Roles != null)
            {
                foreach (var userRole in userDto.Roles)
                {
                    var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == userRole.RoleId);
                    if (existingRole != null)
                    {
                        user.Roles.Add(existingRole);
                    }
                }
            }

            if (!user.Roles.Any())
            {
                user.Roles.Add(await GetDefaultCustomerRoleAsync());
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        private async Task<Role> GetDefaultCustomerRoleAsync()
        {
            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Customer");
            if (customerRole == null)
            {
                customerRole = new Role { RoleName = "Customer" };
                await _context.Roles.AddAsync(customerRole);
                await _context.SaveChangesAsync();
            }
            return customerRole;
        }


        public async Task<string> LoginAsync(UserLoginDTO userDto)
        {
            var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(x => x.Email == userDto.Email);

            if (user == null || !user.Roles.Any())
            {
                throw new UnauthorizedAccessException("Invalid credentials or missing roles.");
            }

            // Convert the stored byte[] (e.g., 0x70617373776F7264) to string ("password")
            string storedPassword = Encoding.UTF8.GetString(user.Password);

            // Check if the provided password matches the stored password text
            if (storedPassword != userDto.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return await GenerateToken(user);
        }

        public async Task<UserDTO> UpdateProfileAsync(int userId, UserUpdateDTO userDto)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _mapper.Map(userDto, user);

            if (userDto.Roles != null && userDto.Roles.Any())
            {
                user.Roles.Clear();

                foreach (var userRole in userDto.Roles)
                {
                    var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == userRole.RoleId);
                    if (existingRole != null)
                    {
                        user.Roles.Add(existingRole);
                    }
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateAddressAsync(int userId, SimpleAddressDTO addressDto)
        {
            var user = await _context.Users.Include(u => u.Orders).ThenInclude(o => o.DeliveryAdress).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var address = _mapper.Map<Address>(addressDto); // Map to Address instead of SimpleAddress
            var order = user.Orders.FirstOrDefault();
            if (order != null)
            {
                if ((DateTime.Now - order.OrderDate).TotalHours > 24)
                {
                    throw new InvalidOperationException("Cannot update address after 24 hours of order placement.");
                }
                order.DeliveryAdress = address;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IList<UserDTO>> GetRolesAsync()
        {
            var users = await _context.Users.Include(u => u.Roles).ToListAsync();
            return _mapper.Map<IList<UserDTO>>(users);

        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            return _mapper.Map<UserDTO>(user);
        }

        private async Task<string> GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var id = await GetClaimsIdentity(user);
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], id.Claims, expires: expires, signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username), // Fix for CS1061: Changed user.UserName to user.Name
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.Now.ToString(CultureInfo.InvariantCulture))
            };

            if (user.Roles != null && user.Roles.Any())
            {
                claims.AddRange(user.Roles.Select(role => new Claim("roleIds", Convert.ToString(role.RoleId))));
                claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.RoleName)));
            }

            return new ClaimsIdentity(claims, "Token");
        }
    }
}

namespace Raktar.DataContext
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hashedPassword) == 0;
        }
    }
}
