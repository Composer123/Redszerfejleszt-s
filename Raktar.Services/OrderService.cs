using AutoMapper;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using Raktar.DataContext;

namespace Raktar.Services
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO);
        Task<OrderDTO> GetOrderByIdAsync(int id);
    }
    public class OrderService : IOrderService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO)
        {
            var order = _mapper.Map<Order>(orderCreateDTO);
            order.OrderDate = DateTime.Now;
            order.Status = OrderStatus.Pending;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            return _mapper.Map<OrderDTO>(order);
        }
    }
}
