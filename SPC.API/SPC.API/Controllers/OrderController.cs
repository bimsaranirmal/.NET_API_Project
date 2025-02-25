using Microsoft.AspNetCore.Mvc;
using SPC.API.Models;
using SPC.API.Services;

namespace SPC.API.Controllers
{
    [ApiController]
    [Route("api/order")]  // Fixed route
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] Order order)
        {
            if (order == null || order.Items == null || order.Items.Count == 0)
            {
                return BadRequest(new { message = "Order must contain at least one item." });
            }

            if (string.IsNullOrEmpty(order.Status))
            {
                return BadRequest(new { message = "Status field is required." });
            }

            foreach (var item in order.Items)
            {
                if (item.DrugId <= 0 || item.Quantity <= 0 || item.UnitPrice <= 0)
                {
                    return BadRequest(new { message = "Invalid order item values." });
                }
            }

            var placedOrder = await _orderService.PlaceOrder(order);
            return Ok(placedOrder);
        }

        // GET: api/order (Get all orders)
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            if (orders == null || orders.Count == 0)
            {
                return NotFound(new { message = "No orders found." });
            }
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            return Ok(order);
        }
        [HttpPost("{orderId}/approve")]
        public async Task<IActionResult> ApproveOrder(int orderId)
        {
            try
            {
                var updatedOrder = await _orderService.UpdateOrderStatus(orderId, "Approved");
                return Ok(new { message = "Order approved successfully.", order = updatedOrder });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error approving order.", error = ex.Message });
            }
        }


        [HttpPost("{orderId}/reject")]
        public async Task<IActionResult> RejectOrder(int orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            if (order.Status != "Pending")
            {
                return BadRequest(new { message = "Only pending orders can be rejected." });
            }

            var updatedOrder = await _orderService.UpdateOrderStatus(orderId, "Rejected");
            return Ok(updatedOrder);
        }



        [HttpGet("order")]
        public async Task<IActionResult> GetOrdersByPharmacyId([FromQuery] int pharmacyId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByPharmacyId(pharmacyId);
                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "No orders found for this pharmacy." });
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}
