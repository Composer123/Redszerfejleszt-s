using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raktar.DataContext;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using Raktar.Services;

namespace Raktar.Services
{
    public interface IAddressService
    {
        /// <summary>
        /// Returns an interface that can be one of two types. Use pattern matching (<see langword="is"/> keyword) to determine which.
        /// <para><seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns"/></para>
        /// </summary>
        /// <returns>Either a <see cref="LandRegistryNumberDTO"/> or <see cref="SimpleAddressDTO"/>.</returns>
        public Task<IAddressDTO> GetAddressAsync(int addressId);
        /// <summary>
        /// Returns an interface that can be one of two types. Use pattern matching (<see langword="is"/> keyword) to determine which.
        /// <para><seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns"/></para>
        /// </summary>
        /// <returns>Either a <see cref="LandRegistryNumberDTO"/> or <see cref="SimpleAddressDTO"/>.</returns>
        public Task<IAddressDTO> CreateAddressAsync(IAddressCreateDTO createDTO);
    }
    public class AddressService(WarehouseDbContext context, IMapper mapper, ILogger<IAddressService> logger) : IAddressService
    {
        private readonly WarehouseDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IAddressDTO> CreateAddressAsync(IAddressCreateDTO createDTO)
        {
            ArgumentNullException.ThrowIfNull(createDTO);

            Address address = new();
            await _context.Addresses.AddAsync(address);

            switch (createDTO)
            {
                case SimpleAddressDTO dto:
                    SimpleAddress sa = _mapper.Map<SimpleAddress>(dto);
                    await _context.SimpleAddresses.AddAsync(sa);
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Created address was a simple address.");
                    break;
                case LandRegistryNumberCreateDTO dto:
                    LandRegistryNumber lrn = _mapper.Map<LandRegistryNumber>(dto);
                    await _context.LandRegistryNumbers.AddAsync(lrn);
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Created address was a land registry number.");
                    break;
                default:
                    throw new ArgumentException("Unexpected address type. Cannot create.");
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<IAddressDTO>(address);
        }
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IAddressDTO> GetAddressAsync(int addressId)
        {
            // Bad solution. Should make address entities with proper inheritance.
            dynamic? tmp = null;

            tmp ??= await _context.LandRegistryNumbers.FindAsync(addressId);
            tmp ??= await _context.SimpleAddresses.FindAsync(addressId);
            
            IAddressDTO r = tmp switch
            {
                SimpleAddress simple => _mapper.Map<SimpleAddressDTO>(simple),
                LandRegistryNumber registryNumber => _mapper.Map<LandRegistryNumberDTO>(registryNumber),
                _ => throw new InvalidOperationException($"No address exists with id #{addressId}"),
            };
            return r;
        }
    }
}
