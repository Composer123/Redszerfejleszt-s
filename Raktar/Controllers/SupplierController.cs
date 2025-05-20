using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using Raktar.Services;

namespace Raktar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Supplier")]
    public class SupplierController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public SupplierController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Change the order's status to ReadyForDelivery and update delivery date
        /// </summary>
        [HttpPut("order/{id}/status")]
        public async Task<IActionResult> ChangeOrderStatus(int id, [FromBody] OrderStatusDTO newStatus)
        {
            if (newStatus.OrderStatus != OrderStatus.ReadyForDelivery)
            {
                return Forbid("Suppliers can only set status to ReadyForDelivery.");
            }

            if (newStatus.DelliveryDate is null)
            {
                return BadRequest("Delivery date is required when setting status to ReadyForDelivery.");
            }

            bool success = await _orderService.ChangeStatusAsync(id, newStatus);
            if (!success)
            {
                return NotFound($"Order with ID {id} was not found.");
            }

            return Ok();
        }

        /// <summary>
        /// Update only the delivery date of an order
        /// </summary>
        [HttpPut("order/{id}/delivery-date")]
        public async Task<IActionResult> UpdateDeliveryDate(int id, [FromBody] OrderStatusDTO deliveryDateUpdate)
        {
            if (deliveryDateUpdate.DelliveryDate is null)
            {
                return BadRequest("Delivery date is required.");
            }

            if (deliveryDateUpdate.OrderStatus != OrderStatus.ReadyForDelivery)
            {
                return BadRequest("Only ReadyForDelivery status is allowed when updating delivery date.");
            }

            bool success = await _orderService.ChangeStatusAsync(id, deliveryDateUpdate);
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