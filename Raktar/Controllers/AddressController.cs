using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Raktar.Services;
using Raktar.DataContext.DataTransferObjects;
using System.Threading.Tasks;

namespace Raktar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Adjust this if needed (or remove it for public endpoints)
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>
        /// Adds a new address to the database.
        /// </summary>
        /// <param name="dto">The address information to create a new Address.</param>
        /// <returns>The created address.</returns>
        [HttpPost]
        public async Task<ActionResult<SimpleAddressDTO>> AddAddress([FromBody] SimpleAddressCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newAddress = await _addressService.CreateAddressAsync(dto);
            return CreatedAtAction(nameof(GetAddressById), new { id = newAddress.AddressId }, newAddress);
        }

        /// <summary>
        /// Retrieves an address by its identifier.
        /// </summary>
        /// <param name="id">The address ID.</param>
        /// <returns>The requested address if found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SimpleAddressDTO>> GetAddressById(int id)
        {
            var address = await _addressService.GetAddressAsync(id);
            if (address == null)
            {
                return NotFound($"Address with ID {id} was not found.");
            }

            return Ok(address);
        }
    }
}
