using AutoMapper;
using Raktar.DataContext;
using Raktar.DataContext.Dtos;
using Raktar.DataContext.Entities;

namespace Raktar.Services
{
    public interface IUserService
    {
        Task<UserDTO> RegisterAsync(UserRegisterDTO userDto);
        Task<string> LoginAsync(UserLoginDTO userDto);
        Task<UserDTO> UpdateProfileAsync(int userId, UserUpdateDTO userDto);
        //Task<UserDTO> UpdateAddressAsync(int userId, AddressDTO addressDto);
        //Task<IList<UserDTO>> GetRolesAsync();
    }
    class UserService
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
            //user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            user.Roles = new List<Role>();

            if (userDto.Roles != null)
            {
                foreach (var roleId in userDto.Roles)
                {
                    var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
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
            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
            if (customerRole == null)
            {
                customerRole = new Role { Name = "Customer" };
                await _context.Roles.AddAsync(customerRole);
                await _context.SaveChangesAsync();
            }
            return customerRole;
        }

        public async Task<string> LoginAsync(UserLoginDTO userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userDto.Email);
            if (user == null /*|| !BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash)*/)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            //return _jwtService.GenerateToken(user);
            return user.Name;
        }

        public async Task<UserDTO> UpdateProfileAsync(int userId, UserUpdateDTO userDto)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _mapper.Map(userDto, user);

            if (userDto.Roles != null && userDto.Roles.Any())
            {
                user.Roles.Clear();

                foreach (var roleId in userDto.Roles)
                {
                    var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
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

    }
}
