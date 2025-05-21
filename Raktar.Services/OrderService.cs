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
        Task<OrderDTO> GetOrderByIdAsync(int id);
        Task<bool> ChangeStatusAsync(int orderId, OrderStatusDTO status);
        Task<IEnumerable<OrderDTO>> GetOrdersUndeliveredAsync();
        Task<IEnumerable<OrderDTO>> GetUndeliveredOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO> ChangeOrderAsync(int orderId, ChangeOrderDTO changeOrderDto);
        Task<OrderDTO> AdminChangeOrderAsync(int orderId, ChangeOrderAdminDTO changeOrderDto);
        Task<OrderDTO> ChangeDeliveryDateAsync(int orderId, DateTime newDeliveryDate);
        //Task<OrderDTO> AssignCarrier(int orderId, AssignOrderCarrierDTO dto);
        Task<bool> AssignCarrierAsync(int orderId, int carrierId);
        Task<IEnumerable<UserDTO>> GetTransportersAsync();
        Task<IEnumerable<OrderDTO>> GetOrdersForCarrierAsync(int carrierId);

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
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Ensure the Product navigation property is loaded
                .Include(o => o.DeliveryAdress)
                    .ThenInclude(a => a.SimpleAddress)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                throw new KeyNotFoundException();

            return _mapper.Map<OrderDTO>(order);
        }



        public async Task<bool> ChangeStatusAsync(int orderId, OrderStatusDTO status)
        {
            Order? order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order is null)
                return false;

            //// 1. Validáció: ne lehessen nulla termék
            //if (order.OrderItems == null || !order.OrderItems.Any())
            //    throw new InvalidOperationException("Order must contain at least one product.");

            //// 2. Validáció: csak bizonyos státuszok engedélyezettek
            //var allowedStatuses = new[] { OrderStatus.Delivered, OrderStatus.Cancelled, OrderStatus.Undeliverible };
            //if (!allowedStatuses.Contains(status.OrderStatus))
            //    throw new InvalidOperationException("Only Delivered, Cancelled, or Undeliverible statuses are allowed.");

            order.Status = status.OrderStatus;
            if (status.DelliveryDate is not null)
                order.DeliveryDate = status.DelliveryDate;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<OrderDTO> ChangeOrderAsync(int orderId, ChangeOrderDTO changeOrderDto)
        {
            // Load the order with its order items and delivery address (including nested SimpleAddress).
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryAdress)
                    .ThenInclude(a => a.SimpleAddress)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            // Only allow changes if the order was placed less than 24 hours ago.
            if ((DateTime.Now - order.OrderDate).TotalHours >= 24)
                throw new InvalidOperationException("Order can only be changed within 24 hours after placement.");

            // Validate that there is at least one product.
            if (changeOrderDto.NewItems == null || !changeOrderDto.NewItems.Any())
                throw new InvalidOperationException("Order must contain at least one product.");

            // --- Update Order Items ---
            // Build a lookup for the new items by product ID.
            var newItemsDict = changeOrderDto.NewItems.ToDictionary(x => x.ProductId);

            // Remove any existing order items that are not in the new list.
            var itemsToRemove = order.OrderItems.Where(oi => !newItemsDict.ContainsKey(oi.ProductId)).ToList();
            foreach (var item in itemsToRemove)
            {
                _context.OrderItems.Remove(item);
            }

            // Update quantities for existing items or add new ones.
            foreach (var newItemDto in changeOrderDto.NewItems)
            {
                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == newItemDto.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity = newItemDto.Quantity;
                }
                else
                {
                    var product = await _context.Products.FindAsync(newItemDto.ProductId)
                        ?? throw new KeyNotFoundException($"Product not found with id {newItemDto.ProductId}.");

                    var newOrderItem = _mapper.Map<OrderItem>(newItemDto);
                    newOrderItem.OrderId = orderId;
                    await _context.OrderItems.AddAsync(newOrderItem);
                }
            }

            // --- Update Delivery Address if Provided ---
            if (changeOrderDto.UpdatedAddress != null)
            {
                // Map the DTO to a SimpleAddress.
                var simpleAddress = _mapper.Map<SimpleAddress>(changeOrderDto.UpdatedAddress);
                // Create a new Address instance containing the SimpleAddress.
                var newAddress = new Address
                {
                    SimpleAddress = simpleAddress
                };

                // Replace the order's current delivery address.
                order.DeliveryAdress = newAddress;
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<OrderDTO>(order);
        }


        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.DeliveryAdress)
                    .ThenInclude(a => a.SimpleAddress)
                .OrderBy(a => a.Status)
                .ThenBy(a => a.OrderDate)
                    .ToListAsync();
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> AdminChangeOrderAsync(int orderId, ChangeOrderAdminDTO changeOrderDto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryAdress)
                    .ThenInclude(a => a.SimpleAddress)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            // Remove the 24-hour check so admin can change any order.
            if (changeOrderDto.NewItems == null || !changeOrderDto.NewItems.Any())
                throw new InvalidOperationException("Order must contain at least one product.");

            // Update order items:
            var newItemsDict = changeOrderDto.NewItems.ToDictionary(x => x.ProductId);
            var itemsToRemove = order.OrderItems.Where(oi => !newItemsDict.ContainsKey(oi.ProductId)).ToList();
            foreach (var item in itemsToRemove)
            {
                _context.OrderItems.Remove(item);
            }

            foreach (var newItemDto in changeOrderDto.NewItems)
            {
                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == newItemDto.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity = newItemDto.Quantity;
                }
                else
                {
                    var product = await _context.Products.FindAsync(newItemDto.ProductId)
                        ?? throw new KeyNotFoundException($"Product not found with id {newItemDto.ProductId}.");
                    var newOrderItem = _mapper.Map<OrderItem>(newItemDto);
                    newOrderItem.OrderId = orderId;
                    await _context.OrderItems.AddAsync(newOrderItem);
                }
            }

            // Update delivery address if the admin opts to change it.
            if (changeOrderDto.UpdatedAddress != null)
            {
                var simpleAddress = _mapper.Map<SimpleAddress>(changeOrderDto.UpdatedAddress);
                var newAddress = new Address { SimpleAddress = simpleAddress };
                order.DeliveryAdress = newAddress;
            }

            if (changeOrderDto.CarrierId is not null)
                order.CarrierId = changeOrderDto.CarrierId;

            // Note: We do not modify order.UserId. It remains as originally set.
            await _context.SaveChangesAsync();
            return _mapper.Map<OrderDTO>(order);
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

        public async Task<OrderDTO> ChangeDeliveryDateAsync(int orderId, DateTime newDeliveryDate)
        {
            // Retrieve the order from the database.
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with id {orderId} was not found.");
            }

            // Update the delivery date.
            order.DeliveryDate = newDeliveryDate;

            // Save changes to the database.
            await _context.SaveChangesAsync();

            // Return the updated order mapped to your OrderDTO.
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<bool> AssignCarrierAsync(int orderId, int carrierId)
        {
            // Get the order by its ID
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            // Load the potential carrier including their Roles
            var carrier = await _context.Users
                .Include(u => u.Roles)  // Assumes User has a Roles navigation property
                .FirstOrDefaultAsync(u => u.UserId == carrierId);

            if (carrier == null)
                throw new Exception("Carrier not found.");

            // Check if the selected user has a Transporter role.
            if (!carrier.Roles.Any(r => r.RoleName == "Transporter"))
                throw new Exception("The selected user is not a valid transporter.");

            // Assign the carrier to the order (and optionally change its status)
            order.CarrierId = carrierId;
            // For example: order.Status = OrderStatus.Processing;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserDTO>> GetTransportersAsync()
        {
            var transporters = await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Roles.Any(r => r.RoleName == "Transporter"))
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDTO>>(transporters);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersForCarrierAsync(int carrierId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryAdress)
                    .ThenInclude(a => a.SimpleAddress)
                .Where(o => o.CarrierId == carrierId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

    }
}
