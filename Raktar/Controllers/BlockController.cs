using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raktar.DataContext.DataTransferObjects;
using Raktar.Services;

namespace Raktar.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class BlockController(IBlockService blockService) : ControllerBase
{
    private readonly IBlockService _blockService = blockService;

    [HttpGet("storage/stock/{productId}")]

    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetStock(int productId)
    {
        int r = await _blockService.GetStockInAllBlocksOfAsync(productId);
        return Ok(r);
    }

    /// <summary>
    /// Assign blocks to storage.
    /// </summary>
    /// <param name="blockDTO">DTO containing quantity and the item.</param>
    [HttpPut("storage/assign")]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Remove a specified amount of item from the storages.
    /// </summary>
    /// <param name="removeDTO">DTO containing item and amount.</param>
    [HttpPut("storage/remove")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveFromStorage([FromBody] BlockAssignOrRemoveDTO removeDTO)
    {
        bool r = await _blockService.TryRemoveProductAsync(removeDTO);
        if (r)
            return Ok();

        return BadRequest("Product could not be removed from blocks.");
    }
}
