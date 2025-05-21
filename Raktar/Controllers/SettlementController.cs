using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Raktar.DataContext;
using Raktar.DataContext.Entities;
using Raktar.DataContext.DataTransferObjects; // Make sure your DTOs are here
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Raktar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Remove or adjust if public access is OK.
    public class SettlementController : ControllerBase
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public SettlementController(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all available settlements.
        /// </summary>
        /// <returns>List of SettlementDTO</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SettlementDTO>>> GetSettlements()
        {
            // Retrieve all settlements from the database.
            var settlements = await _context.Settlements.ToListAsync();

            // Map to your SettlementDTO so the client receives the required fields.
            var settlementDtos = _mapper.Map<List<SettlementDTO>>(settlements);
            return Ok(settlementDtos);
        }
    }
}
