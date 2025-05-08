using AutoMapper;
using Raktar.DataContext;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace Raktar.Services
{
    public interface IProductService
    {
        Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDTO);
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    }
    public class ProductService : IProductService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDTO)
        {
            var product = _mapper.Map<Product>(productCreateDTO);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            return await _context.Products
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Type = p.Type,
                    Price = p.Price,
                    MaxQuantityPerBlock = p.MaxQuantityPerBlock
                })
                .ToListAsync();
        }
    }
}
