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
        /// <returns>False if no empty block are found or given quantity can not be fitted into already semi-filled blocks. Otherwise true.</returns>
        public Task<bool> TryAssignProductToBlockAsync(BlockAssignDTO assignment);
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
                throw new InvalidOperationException($"Couldn't find given product({productId}).");

            int stock = containerBlocks.Sum(b => b.Quantity);
            return stock;
        }

        public async Task<bool> TryAssignProductToBlockAsync(BlockAssignDTO assignment)
        {
            //Product product = await _context.Products.FindAsync(productId)
            //    ?? throw new InvalidOperationException($"No product with id {productId} exists.");

            Block[] validBlocks = await _context.Blocks
                .Where(b => b.ProductId == assignment.Item.ProductId || b.ProductId == null)
                .ToArrayAsync();

            int maxCapacity = assignment.Item.MaxQuantityPerBlock * validBlocks.Length - validBlocks.Sum(b => b.Quantity);
            if (assignment.Quantity > maxCapacity)
                return false;

            int nonAssigned = assignment.Quantity;
            for (int i = 0; i < validBlocks.Length && nonAssigned > 0; i++)
            {
                validBlocks[i].ProductId ??= assignment.Item.ProductId;

                int curentlyAssigned = Math.Min(nonAssigned, assignment.Item.MaxQuantityPerBlock);
                curentlyAssigned -= validBlocks[i].Quantity;
                validBlocks[i].Quantity += curentlyAssigned;

                nonAssigned -= curentlyAssigned;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
