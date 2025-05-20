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
        //public async Task<IAddressDTO> CreateAddressAsync(IAddressCreateDTO createDTO)
        //{
        //    ArgumentNullException.ThrowIfNull(createDTO);

        //    Address address = new();
        //    await _context.Addresses.AddAsync(address);

        //    IAddressDTO r = null!;
        //    switch (createDTO)
        //    {
        //        case SimpleAddressDTO dto:
        //            SimpleAddress sa = _mapper.Map<SimpleAddress>(dto);
        //            await _context.SimpleAddresses.AddAsync(sa);
        //            r = _mapper.Map<SimpleAddressDTO>(sa);
        //            if (_logger.IsEnabled(LogLevel.Debug))
        //                _logger.LogDebug("Created address was a simple address.");
        //            break;
        //        case LandRegistryNumberCreateDTO dto:
        //            LandRegistryNumber lrn = _mapper.Map<LandRegistryNumber>(dto);
        //            await _context.LandRegistryNumbers.AddAsync(lrn);
        //            r = _mapper.Map<LandRegistryNumberDTO>(lrn);
        //            if (_logger.IsEnabled(LogLevel.Debug))
        //                _logger.LogDebug("Created address was a land registry number.");
        //            break;
        //        default:
        //            throw new ArgumentException("Unexpected address type. Cannot create.");
        //    }

        //    await _context.SaveChangesAsync();
        //    return r;
        //}

        public async Task<IAddressDTO> CreateAddressAsync(IAddressCreateDTO createDTO)
        {
            ArgumentNullException.ThrowIfNull(createDTO);

            // Create the container Address
            Address address = new();
            await _context.Addresses.AddAsync(address);

            IAddressDTO result = null!;
            switch (createDTO)
            {
                case SimpleAddressCreateDTO dto:
                    var existingSettlement = await _context.Settlements.FirstOrDefaultAsync(s =>
                        s.PostCode == dto.Settlement.PostCode &&
                        s.SettlementName == dto.Settlement.SettlementName);

                    if (existingSettlement == null)
                    {
                        throw new ArgumentException("The provided Settlement does not exist in the database.");
                    }

                    Settlement settlement = existingSettlement;
                    SimpleAddress simpleAddress = _mapper.Map<SimpleAddress>(dto);
                    simpleAddress.Settlement = settlement;
                    simpleAddress.SettlementId = settlement.SettlementId;
                    simpleAddress.Address = address;
                    await _context.SimpleAddresses.AddAsync(simpleAddress);

                    result = _mapper.Map<SimpleAddressDTO>(simpleAddress);
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Created address was a simple address using an existing settlement.");
                    break;

                case LandRegistryNumberCreateDTO dto:
                    LandRegistryNumber lrn = _mapper.Map<LandRegistryNumber>(dto);
                    await _context.LandRegistryNumbers.AddAsync(lrn);
                    result = _mapper.Map<LandRegistryNumberDTO>(lrn);
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Created address was a land registry number.");
                    break;

                default:
                    throw new ArgumentException("Unexpected address type. Cannot create.");
            }

            await _context.SaveChangesAsync();

            // Ensure the new container gets the generated AddressId.
            await _context.Entry(address).ReloadAsync();

            // Optionally update the SimpleAddress entity if needed:
            if (result is SimpleAddressDTO simpleDto)
            {
                simpleDto.AddressId = address.AddressId;
            }

            return result;
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
