using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.DTOs;
using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryDeliveryAPI.Managers
{
    public class OrderManager
    {
        private readonly GroceryDeliveryContext _context;
        private readonly DeliveryManager deliveryManager;

        public OrderManager(GroceryDeliveryContext context, DeliveryManager deliveryManager)
        {
            _context = context;
            this.deliveryManager = deliveryManager;
        }

        public async Task<(Order Order, string Error)> CreateOrderAsync(OrderCreateDTO orderDto)
        {
            var user = await _context.Users.FindAsync(orderDto.UserId);
            if (user == null)
                return (null, $"User with ID {orderDto.UserId} not found.");

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in orderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                    return (null, $"Product with ID {itemDto.ProductId} not found.");

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
                Address = orderDto.Address,
                City = orderDto.City,
                ZipCode = orderDto.ZipCode,
                Country = orderDto.Country,
                TotalAmount = totalAmount,
                Status = "Pending",
                PaymentMethod = orderDto.PaymentMethod,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();


            await deliveryManager.CreateDelivery(order.OrderId);

            return (order, null);
        }

        public async Task<List<OrderDTO>> GetOrdersForUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return null;

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapOrderToDTO).ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderAsync(int orderId, OrderUpdateDTO updateDto)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            order.Address = updateDto.Address;
            order.City = updateDto.City;
            order.ZipCode = updateDto.ZipCode;
            order.Country = updateDto.Country;
            order.PaymentMethod = updateDto.PaymentMethod;
            order.DeliveryTime = updateDto.DeliveryTime;

            await _context.SaveChangesAsync();
            return true;
        }
        /*
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }*/

        public async Task CancelOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                throw new InvalidOperationException($"Order with ID {id} not found.");

            order.Status = "Cancelled";

            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            // Find delivery associated with order
            var delivery = await _context.Deliveries
    .Include(d => d.DeliveryPerson)
    .FirstOrDefaultAsync(d => d.OrderId == id);
            if (delivery != null)
            {
                delivery.Status = Delivery.DeliveryStatus.Cancelled;
                delivery.PickupTime = null;
                delivery.EstimatedDeliveryTime = null;
                delivery.DeliveryPerson.Status = DeliveryPerson.DeliveryPersonStatus.Available;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<(List<OrderDTO> Orders, int TotalCount)> GetAllOrdersAsync(
            int page = 1,
            int pageSize = 10,
            string status = null,
            int? userId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
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

            return (orders.Select(MapOrderToDTO).ToList(), totalCount);
        }

        private static OrderDTO MapOrderToDTO(Order order)
        {
            return new OrderDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Address = order.Address,
                City = order.City,
                ZipCode = order.ZipCode,
                Country = order.Country,
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
            };
        }
    }
}