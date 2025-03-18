using Microsoft.AspNetCore.Mvc;
using Raktar.DataContext.DataTransferObjects;
using Raktar.Services;

namespace Raktar.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockController(IBlockService blockService) : ControllerBase
{
    private readonly IBlockService _blockService = blockService;

    [HttpGet("storage/stock/{productId}")]
    public async Task<IActionResult> GetStock(int productId)
    {
        int r = await _blockService.GetStockInAllBlocksOfAsync(productId);
        return Ok(r);
    }

    [HttpPost("storage/assign")]
    public async Task<IActionResult> AssignStorage([FromBody] BlockAssignDTO blockDTO)
    {
        bool r = await _blockService.TryAssignProductToBlockAsync(blockDTO);
        if (r)
            Ok();

        return BadRequest("Product could not be assigned to any blocks.");
    }
}
