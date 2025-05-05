using Microsoft.AspNetCore.Mvc;
using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.Models;
using GroceryDeliveryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GroceryDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly GroceryDeliveryContext _context;
        public OrderController(GroceryDeliveryContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if user exists
            var user = await _context.Users.FindAsync(orderDto.UserId);
            if (user == null)
                return NotFound($"User with ID {orderDto.UserId} not found.");

            // Prepare order items and calculate total
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();
            foreach (var itemDto in orderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                    return NotFound($"Product with ID {itemDto.ProductId} not found.");

                decimal subtotal = product.Price * itemDto.Quantity;
                totalAmount += subtotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = subtotal
                });
            }

            var order = new Order
            {
                UserId = orderDto.UserId,
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = orderDto.DeliveryAddress,
                TotalAmount = totalAmount,
                Status = "Pending",
                PaymentMethod = orderDto.PaymentMethod,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(null, new { orderId = order.OrderId }, new { order.OrderId, order.TotalAmount, order.Status });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersForUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound($"User with ID {userId} not found.");

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderDTOs = orders.Select(order => new OrderDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                DeliveryTime = order.DeliveryTime,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.ProductName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal
                }).ToList()
            }).ToList();

            return Ok(orderDTOs);
        }

        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] OrderStatusUpdateDTO statusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound($"Order with ID {orderId} not found.");

            order.Status = statusDto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { order.OrderId, order.Status });
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderUpdateDTO updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound($"Order with ID {orderId} not found.");

            order.DeliveryAddress = updateDto.DeliveryAddress;
            order.PaymentMethod = updateDto.PaymentMethod;
            order.DeliveryTime = updateDto.DeliveryTime;

            await _context.SaveChangesAsync();

            return Ok(new { order.OrderId, order.DeliveryAddress, order.PaymentMethod, order.DeliveryTime });
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return NotFound($"Order with ID {orderId} not found.");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string status = null,
            [FromQuery] int? userId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);
            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId);
            if (fromDate.HasValue)
                query = query.Where(o => o.OrderDate >= fromDate);
            if (toDate.HasValue)
                query = query.Where(o => o.OrderDate <= toDate);

            int totalCount = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var orderDTOs = orders.Select(order => new OrderDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                DeliveryTime = order.DeliveryTime,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.ProductName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal
                }).ToList()
            }).ToList();

            return Ok(new {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Orders = orderDTOs
            });
        }
    }
} 