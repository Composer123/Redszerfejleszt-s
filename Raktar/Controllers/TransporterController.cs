using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using Raktar.Services;

namespace Raktar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Transporter")]
    public class TransporterController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public TransporterController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Change the order's status. Only allows Delivered, Cancelled, or Undeliverable statuses.
        /// </summary>
        [HttpPut("order/{id}/status")]
        public async Task<IActionResult> ChangeOrderStatus(int id, [FromBody] OrderStatusDTO newStatus)
        {
            var allowedStatuses = new[] { OrderStatus.Delivered, OrderStatus.Cancelled, OrderStatus.Undeliverible };
            if (!allowedStatuses.Contains(newStatus.OrderStatus))
            {
                return Forbid("Transporters can only set status to Delivered, Cancelled, or Undeliverable.");
            }

            if (newStatus.DelliveryDate is not null)
            {
                return Forbid("Transporters cannot modify delivery dates.");
            }

            bool success = await _orderService.ChangeStatusAsync(id, newStatus);
            if (!success)
            {
                return NotFound($"Order with ID {id} was not found.");
            }

            return Ok();
        }

        /// <summary>
        /// Get all undelivered orders
        /// </summary>
        [HttpGet("orders/undelivered")]
        public async Task<IActionResult> GetUndeliveredOrders()
        {
            var orders = await _orderService.GetOrdersUndeliveredAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Get undelivered orders for a specific user
        /// </summary>
        [HttpGet("orders/undelivered/{userId}")]
        public async Task<IActionResult> GetUndeliveredOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetUndeliveredOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
    }
} 