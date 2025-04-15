using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using Raktar.Services;

namespace Raktar.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]

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

        // GET: api/order/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeOrderStatus(int id)
        {
            bool succes = await _orderService.ChangeStatusAsync(id, OrderStatus.Delivered);

            if (!succes)
                return NotFound($"Order with ID {id} not found.");

            return Ok();
        }

        [HttpGet("delivery")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUndeliveredOrders()
        {
            var r = await _orderService.GetOrdersUndeliveredAsync();
            return Ok(r);
        }
    }
}
