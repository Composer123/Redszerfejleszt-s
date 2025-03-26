
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Raktar.DataContext;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using System.Security.Cryptography;

namespace Raktar.Services
{
    public interface IUserService
    {
        Task<UserDTO> RegisterAsync(UserRegisterDTO userDto);
        Task<string> LoginAsync(UserLoginDTO userDto);
        Task<UserDTO> UpdateProfileAsync(int userId, UserUpdateDTO userDto);
        Task<UserDTO> UpdateAddressAsync(int userId, AddressDTO addressDto);
        Task<IList<UserDTO>> GetRolesAsync();
    }

    public class UserService : IUserService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public UserService(
            WarehouseDbContext context,
            IMapper mapper
        )
        {
            _context = context;
            _mapper = mapper;
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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userDto.Email);
            if (user == null || !PasswordHasher.VerifyPassword(userDto.Password, Encoding.UTF8.GetString(user.Password)))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            // return _jwtService.GenerateToken(user);
            return user.Username;
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

        public async Task<UserDTO> UpdateAddressAsync(int userId, AddressDTO addressDto)
        {
            var user = await _context.Users.Include(u => u.Orders).ThenInclude(o => o.DeliveryAdress).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var address = _mapper.Map<Address>(addressDto);
            var order = user.Orders.FirstOrDefault();
            if (order != null)
            {
                order.DeliveryAdress = address;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IList<UserDTO>> GetRolesAsync()
        {
            var users = await _context.Users.Include(u => u.Roles).ToListAsync();
            return _mapper.Map<IList<UserDTO>>(users);
        }
    }
}


//using System.Text;

namespace Raktar.Services
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

namespace Raktar.DataContext.DataTransferObjects
{
    public class AddressDTO
    {
        public int AddressId { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public SettlementDTO Settlement { get; set; }
    }
}