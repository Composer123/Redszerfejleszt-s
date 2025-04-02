using AutoMapper;
using Raktar.DataContext;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Raktar.DataContext.DataTransferObjects.ProductServiceDTO;


namespace Raktar.Services
{
    public interface IProductService
    {
            Task<ProductDTO> CreateProductAsync( ProductCreateDTO productCreateDTO);
            Task<ProductDTO> GetProductByIdAsync(int id);
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
    }
}
