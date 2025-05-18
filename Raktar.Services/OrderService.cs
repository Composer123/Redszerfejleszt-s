using AutoMapper;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using Raktar.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Raktar.Services
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO);
        Task<OrderDTO> AddItemToOrderAsync(int orderId,AddOrderItemDTO dto);
        Task<OrderDTO> GetOrderByIdAsync(int id);
        Task<bool> ChangeStatusAsync(int orderId, OrderStatusDTO status);
        Task<IEnumerable<OrderDTO>> GetOrdersUndeliveredAsync();
        Task<IEnumerable<OrderDTO>> GetUndeliveredOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId);


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

        public async Task<OrderDTO> AddItemToOrderAsync(int orderId,AddOrderItemDTO dto)
        {
            Product product = await _context.Products.FindAsync(dto.ProductId)
                ?? throw new KeyNotFoundException($"Product not found with id {dto.ProductId}.");
            Order order = await _context.Orders.FindAsync(orderId)
                ?? throw new KeyNotFoundException($"Order not found with id {orderId}.");

            OrderItem orderItem = _mapper.Map<OrderItem>(dto);
            await _context.OrderItems.AddAsync(orderItem);
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

        public async Task<bool> ChangeStatusAsync(int orderId, OrderStatusDTO status)
        {
            Order? order = await _context.Orders.FindAsync(orderId);
            if (order is null)
                return false;

            order.Status = status.OrderStatus;
            if (status.DelliveryDate is not null )
                order.DeliveryDate = status.DelliveryDate;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersUndeliveredAsync()
        {
            return 
                await _context.Orders
                .Where(o => o.Status == OrderStatus.ReadyForDelivery)
                .Select(o => _mapper.Map<OrderDTO>(o))
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDTO>> GetUndeliveredOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.ReadyForDelivery && o.UserId == userId)
                .Select(o => _mapper.Map<OrderDTO>(o))
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Select(o => _mapper.Map<OrderDTO>(o))
                .ToListAsync();
        }

    }
}
