using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;
using Raktar.Services;
using System.Security.Claims;

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

        /// <summary>
        /// Creating an Order.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] OrderCreateDTO orderCreateDTO)
        {
            var newOrder = await _orderService.CreateOrderAsync(orderCreateDTO);
            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, newOrder);
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Order with ID {id} not found.");
            }
        }
        [HttpGet("admin")]
        [Authorize(Roles = "Admin, Transporter, Supplier")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPut("admin/change/{orderId}")]
        [Authorize(Roles = "Admin,Supplier")]
        public async Task<ActionResult<OrderDTO>> AdminChangeOrder(int orderId, [FromBody] ChangeOrderAdminDTO changeOrderDto)
        {
            try
            {
                var updatedOrder = await _orderService.AdminChangeOrderAsync(orderId, changeOrderDto);
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/order/assign-carrier/{orderId}
        [HttpPut("assign-carrier/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignCarrier(int orderId, [FromBody] AssignOrderCarrierDTO dto)
        {
            try
            {
                var result = await _orderService.AssignCarrierAsync(orderId, dto.CarrierId);
                if (result)
                    return Ok("Carrier assigned successfully.");
                else
                    return BadRequest("Failed to assign carrier.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Change the order's status. Validation implementation needed.
        /// </summary>
        [HttpPut("delivery/{id}")]
        [Authorize]
        public async Task<IActionResult> ChangeOrderStatus(int id, [FromBody] OrderStatusDTO newStatus)
        {

            // If the logged-in user has the role "Customer", they are only allowed to cancel orders.
            if (!User.IsInRole("Admin"))
            {
                bool canCancelOrders = User.IsInRole("Customer");
                bool canDeliverOrders = User.IsInRole("Transporter") && (
                    newStatus.OrderStatus == OrderStatus.Accepted || 
                    newStatus.OrderStatus == OrderStatus.Delivered || 
                    newStatus.OrderStatus == OrderStatus.Cancelled || 
                    newStatus.OrderStatus == OrderStatus.Undeliverible) && newStatus.DelliveryDate is null;
                bool canPrepareOrders = User.IsInRole("Supplier") && (
                    newStatus.OrderStatus == OrderStatus.ReadyForDelivery || 
                    newStatus.OrderStatus == OrderStatus.Cancelled || 
                    newStatus.OrderStatus == OrderStatus.Undeliverible) && newStatus.DelliveryDate is null;

                if (!(canCancelOrders || canDeliverOrders || canPrepareOrders))
                {
                    return Forbid("You do not have the necessary permissions for this action.");
                }
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
        [Authorize] // Use [Authorize] if you require a valid token
        public async Task<IActionResult> GetUndeliveredOrdersByUserId(int userId)
        {
            // Extract the numeric user ID from the token's claims
            string? claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole("Customer") && userId.ToString() != claimUserId)
            {
                return Forbid("You can only view your own undelivered orders.");
            }

            var orders = await _orderService.GetUndeliveredOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            string claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole("Customer") && userId.ToString() != claimUserId)
            {
                return Forbid("You can only view your own orders.");
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
        [HttpPut("change/{orderId}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<OrderDTO>> ChangeOrder(int orderId, [FromBody] ChangeOrderDTO changeOrderDto)
        {
            try
            {
                var updatedOrder = await _orderService.ChangeOrderAsync(orderId, changeOrderDto);
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("change-delivery-date/{orderId}")]
        [Authorize(Roles = "Admin,Transporter")]
        public async Task<IActionResult> ChangeDeliveryDate(int orderId, [FromBody] ChangeDeliveryDateDTO changeDeliveryDateDTO)
        {
            try
            {
                // Call the service method to update the delivery date.
                var updatedOrder = await _orderService.ChangeDeliveryDateAsync(orderId, changeDeliveryDateDTO.NewDeliveryDate);
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return BadRequest($"Error updating delivery date: {ex.Message}");
            }
        }

        [HttpGet("transporters")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetTransporters()
        {
            try
            {
                var transporters = await _orderService.GetTransportersAsync();
                return Ok(transporters);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/order/transporter
        [HttpGet("transporter")]
        [Authorize(Roles = "Transporter")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersForTransporter()
        {
            try
            {
                // Retrieve the authenticated transporter's ID from token claims.
                // Adjust the claim type if necessary (e.g. "sub" or ClaimTypes.NameIdentifier).
                var transporterIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(transporterIdClaim))
                {
                    return Unauthorized("Transporter ID not found in token.");
                }

                if (!int.TryParse(transporterIdClaim, out int transporterId))
                {
                    return BadRequest("Invalid transporter ID.");
                }

                var orders = await _orderService.GetOrdersForCarrierAsync(transporterId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
