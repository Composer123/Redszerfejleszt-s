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

    [HttpPut("storage/assign")]
    public async Task<IActionResult> AssignStorage([FromBody] BlockAssignOrRemoveDTO blockDTO)
    {
        try
        {
            bool r = await _blockService.TryAssignProductToBlockAsync(blockDTO);
            if (r)
                return Ok();

            return BadRequest("Product could not be assigned to any blocks.");
        }
        catch (InvalidOperationException opEx)
        {
            return BadRequest(opEx.Message);
        }
    }

    [HttpPut("storage/remove")]
    public async Task<IActionResult> RemoveFromStorage([FromBody] BlockAssignOrRemoveDTO removeDTO)
    {
        bool r = await _blockService.TryRemoveProductAsync(removeDTO);
        if (r)
            return Ok();

        return BadRequest("Product could not be removed from blocks.");
    }
}
