using Microsoft.AspNetCore.Mvc;
using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.Models;
using GroceryDeliveryAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using GroceryDeliveryAPI.Managers;

namespace GroceryDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderManager _orderManager;

        public OrderController(OrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (order, error) = await _orderManager.CreateOrderAsync(orderDto);

            if (error != null)
                return NotFound(error);

            return CreatedAtAction(nameof(GetOrdersForUser),
                new { userId = order.UserId },
                new { order.OrderId, order.TotalAmount, order.Status });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersForUser(int userId)
        {
            var orders = await _orderManager.GetOrdersForUserAsync(userId);

            if (orders == null)
                return NotFound($"User with ID {userId} not found.");

            return Ok(orders);
        }

        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] OrderStatusUpdateDTO statusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _orderManager.UpdateOrderStatusAsync(orderId, statusDto.Status);

            if (!success)
                return NotFound($"Order with ID {orderId} not found.");

            return Ok(new { OrderId = orderId, Status = statusDto.Status });
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderUpdateDTO updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _orderManager.UpdateOrderAsync(orderId, updateDto);

            if (!success)
                return NotFound($"Order with ID {orderId} not found.");

            return Ok(new { OrderId = orderId, updateDto.Address, updateDto.City, updateDto.ZipCode, updateDto.Country, updateDto.PaymentMethod, updateDto.DeliveryTime });
        }

        [HttpPut("cancel/{orderId}")]

        public async Task<IActionResult> CancelOrder(int orderId)
        {
            await _orderManager.CancelOrder(orderId);

            return NoContent();
        }

        /*
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var success = await _orderManager.DeleteOrderAsync(orderId);

            if (!success)
                return NotFound($"Order with ID {orderId} not found.");

            return NoContent();
        }
        */
        [HttpGet]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string status = null,
            [FromQuery] int? userId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var (orders, totalCount) = await _orderManager.GetAllOrdersAsync(
                page, pageSize, status, userId, fromDate, toDate);

            return Ok(new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Orders = orders
            });
        }
    }
}