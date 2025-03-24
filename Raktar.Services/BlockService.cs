using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raktar.DataContext;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Raktar.Services;

public interface IBlockService
{
    public Task<int> GetStockInAllBlocksOfAsync(int productId);
    /// <summary>
    /// Tries to assign product to an empty storage block.
    /// </summary>
    /// <returns><see langword="false"/> if no empty block are found or given quantity can not be fitted into already semi-filled blocks. Otherwise <see langword="true"/>.</returns>
    public Task<bool> TryAssignProductToBlockAsync(BlockAssignOrRemoveDTO assignmentDTO);
    /// <summary>
    /// Tries to remove a certain product with give quantity. If all items are removed then blocks product is set back to <see langword="null"/>.
    /// </summary>
    /// <returns><see langword="false"/> if give amount is not present. Otherwise <see langword="true"/>.</returns>
    public Task<bool> TryRemoveProductAsync(BlockAssignOrRemoveDTO removeDTO);
}

public class BlockService(WarehouseDbContext context, ILogger<IBlockService> logger) : IBlockService
{
    private readonly WarehouseDbContext _context = context;
    private readonly ILogger _logger = logger;

    /// <exception cref="InvalidOperationException"></exception>
    public async Task<int> GetStockInAllBlocksOfAsync(int productId)
    {
        Block[] containerBlocks =
            await _context.Blocks
            .Where(b => b.ProductId == productId)
            .ToArrayAsync();

        if (containerBlocks.Length == 0)
            return 0;

        int stock = containerBlocks.Sum(b => b.Quantity);
        return stock;
    }

    public async Task<bool> TryAssignProductToBlockAsync(BlockAssignOrRemoveDTO assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        int productId = assignment.Item.ProductId;
        if (await _context.Products.FindAsync(productId) == null)
            throw new InvalidOperationException($"Couldn't find given product #{assignment.Item.ProductId}");

        Block[] validBlocks = await _context.Blocks
            .Where(b => b.ProductId == productId || b.ProductId == null)
            .ToArrayAsync();

        int maxCapacity = assignment.Item.MaxQuantityPerBlock * validBlocks.Length - validBlocks.Sum(b => b.Quantity);
        if (assignment.Quantity > maxCapacity)
            return false;

        await PerformAssignment(ref validBlocks, assignment.Quantity, productId, assignment.Item.MaxQuantityPerBlock, _logger);

        static ValueTask PerformAssignment(ref Block[] blocks, int fullQuantity, int productId, int quantityPerBlock, ILogger logger)
        {
            ref Block searchSpace = ref MemoryMarshal.GetArrayDataReference(blocks);
            int nonAssigned = fullQuantity;
            for (int i = 0; i < blocks.Length && nonAssigned > 0; i++)
            {
                Block currentBlock = Unsafe.Add(ref searchSpace, i);

                currentBlock.ProductId ??= productId;

                int currentlyAssigned = Math.Min(nonAssigned, quantityPerBlock);
                currentlyAssigned -= currentBlock.Quantity;
                currentBlock.Quantity += currentlyAssigned;

                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug("Added {quantity} of product #{productId} to block #{blockId}", currentlyAssigned, productId, currentBlock.BlockId);

                nonAssigned -= currentlyAssigned;
            }

            return new();
        }

        _ = await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryRemoveProductAsync(BlockAssignOrRemoveDTO removeDTO)
    {
        ArgumentNullException.ThrowIfNull(removeDTO);

        Block[] validBlocks = await _context.Blocks
            .Where(b => b.ProductId == removeDTO.Item.ProductId)
            .ToArrayAsync();

        int stock = validBlocks.Sum(b => b.Quantity);
        if (stock < removeDTO.Quantity)
            return false;

        await PerformRemoval(ref validBlocks, fullQuantity: removeDTO.Quantity, quantityPerBlock: removeDTO.Item.MaxQuantityPerBlock, _logger);

        static ValueTask PerformRemoval(ref Block[] blocks, int fullQuantity, int quantityPerBlock, ILogger logger)
        {
            ref Block searchSpace = ref MemoryMarshal.GetArrayDataReference(blocks);
            int remainingQuantity = fullQuantity;
            for (int i = blocks.Length - 1; i >= 0 && remainingQuantity > 0; i--)
            {
                Block currentBlock = Unsafe.Add(ref searchSpace, i);

                int curentlyRemoved = Math.Min(remainingQuantity, quantityPerBlock);
                curentlyRemoved = Math.Min(curentlyRemoved, currentBlock.Quantity);

                currentBlock.Quantity -= curentlyRemoved;
                if (currentBlock.Quantity == 0)
                    currentBlock.ProductId = null;

                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug("Removed {quantity}  from block #{blockId}", curentlyRemoved, currentBlock.BlockId);

                remainingQuantity -= curentlyRemoved;
            }

            return new();
        }

        _ = await _context.SaveChangesAsync();
        return true;
    }
}
