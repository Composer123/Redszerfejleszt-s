using Microsoft.EntityFrameworkCore;
using Raktar.DataContext;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;

namespace Raktar.Services
{
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

    public class BlockService(WarehouseDbContext context) : IBlockService
    {
        private readonly WarehouseDbContext _context = context;

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
            int productId = assignment.Item.ProductId;
            if (await _context.Products.FindAsync(productId) == null)
                throw new InvalidOperationException($"Couldn't find given product #{assignment.Item.ProductId}");

            Block[] validBlocks = await _context.Blocks
                .Where(b => b.ProductId == productId || b.ProductId == null)
                .ToArrayAsync();

            int maxCapacity = assignment.Item.MaxQuantityPerBlock * validBlocks.Length - validBlocks.Sum(b => b.Quantity);
            if (assignment.Quantity > maxCapacity)
                return false;

            int nonAssigned = assignment.Quantity;
            for (int i = 0; i < validBlocks.Length && nonAssigned > 0; i++)
            {
                validBlocks[i].ProductId ??= productId;

                int curentlyAssigned = Math.Min(nonAssigned, assignment.Item.MaxQuantityPerBlock);
                curentlyAssigned -= validBlocks[i].Quantity;
                validBlocks[i].Quantity += curentlyAssigned;

                nonAssigned -= curentlyAssigned;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TryRemoveProductAsync(BlockAssignOrRemoveDTO removeDTO)
        {
            Block[] validBlocks = await _context.Blocks
                .Where(b => b.ProductId == removeDTO.Item.ProductId)
                .ToArrayAsync();

            int stock = validBlocks.Sum(b => b.Quantity);
            if (stock < removeDTO.Quantity)
                return false;

            int remainingQuantity = removeDTO.Quantity;
            for (int i = validBlocks.Length - 1; i >= 0 && remainingQuantity > 0; i--)
            {
                int curentlyRemoved = Math.Min(remainingQuantity, removeDTO.Item.MaxQuantityPerBlock);
                curentlyRemoved = Math.Min(curentlyRemoved, validBlocks[i].Quantity);

                validBlocks[i].Quantity -= curentlyRemoved;
                if (validBlocks[i].Quantity == 0)
                    validBlocks[i].ProductId = null;

                remainingQuantity -= curentlyRemoved;
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
