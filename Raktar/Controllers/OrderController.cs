using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using Raktar.Services;

namespace Raktar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/order
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] OrderCreateDTO orderCreateDTO)
        {
            var newOrder = await _orderService.CreateOrderAsync(orderCreateDTO);
            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, newOrder);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<OrderDTO>> AddToOrder([FromRoute] int id, [FromBody] AddOrderItemDTO addOrderItemDTO)
        {
            var order = await _orderService.AddItemToOrderAsync(id, addOrderItemDTO);
            return Ok(order);
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Order with ID {id} not found.");
            }
        }

        [HttpPut("delivery/{id}")]
        [Authorize]
        public async Task<IActionResult> ChangeOrderStatus(int id, [FromBody] IOrderStatusDTO newStatus)
        {
            // If the logged-in user has the role "Customer", they are only allowed to cancel orders.
            if (User.IsInRole("Customer") && newStatus is not OrderCancelStatusDTO)
            {
                return Forbid("Users with the 'User' role can only cancel orders.");
            }

            bool success = await _orderService.ChangeStatusAsync(id, newStatus);
            if (!success)
            {
                return NotFound($"Order with ID {id} was not found.");
            }

            return Ok();
        }

        [HttpGet("delivery")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUndeliveredOrders()
        {
            var r = await _orderService.GetOrdersUndeliveredAsync();
            return Ok(r);
        }

        [HttpGet("delivery/user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUndeliveredOrdersByUserId(int userId)
        {
            // Ensure the logged-in user can only fetch their own orders (unless admin)
            if (User.IsInRole("Customer") && userId.ToString() != User.Identity?.Name)
            {
                return Forbid("You can only view your own undelivered orders.");
            }

            var orders = await _orderService.GetUndeliveredOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            // Ensure customers can only fetch their own orders unless they are an admin
            if (User.IsInRole("Customer") && userId.ToString() != User.Identity?.Name)
            {
                return Forbid("You can only view your own orders.");
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

    }
}
