using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.DTO_s;
using GroceryDeliveryAPI.DTOs;
using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GroceryDeliveryAPI.Managers
{
    public class DeliveryManager
    {
        private GroceryDeliveryContext _context;

        public DeliveryManager(GroceryDeliveryContext context)
        {
            _context = context;
        }

        public async Task<List<Delivery>> GetAllDeliveries()
        {
            var deliveries = await _context.Deliveries.Select(d => new Delivery
            {
                DeliveryId = d.DeliveryId,
                DeliveryPersonId = d.DeliveryPersonId,
                OrderId = d.OrderId,
                Status = d.Status,
                PickupTime = d.PickupTime,
                EstimatedDeliveryTime = d.EstimatedDeliveryTime,
                DeliveryPerson = d.DeliveryPerson,
                Order = d.Order
            }).ToListAsync();
            return deliveries;
        }

        public async Task<Delivery> GetDeliveryById(int id)
        {
            //var delivery = await _context.Deliveries.FindAsync(id);

            var delivery = await _context.Deliveries.Select(d => new Delivery
            {
                DeliveryId = d.DeliveryId,
                DeliveryPersonId = d.DeliveryPersonId,
                OrderId = d.OrderId,
                Status = d.Status,
                PickupTime = d.PickupTime,
                EstimatedDeliveryTime = d.EstimatedDeliveryTime,
                DeliveryPerson = d.DeliveryPerson,
                Order = d.Order
            }).FirstOrDefaultAsync(d => d.DeliveryId == id);
           
            return delivery;
        }

        // Get Active delivery by delivery person id
        public async Task<List<DeliveryDTO>> GetDeliveriesByDeliveryPersonId(int deliveryPersonId)
        {
            var deliveries = await _context.Deliveries
                .Where(d => d.DeliveryPersonId == deliveryPersonId)
                .Where(d => d.Status == Delivery.DeliveryStatus.Pending || d.Status == Delivery.DeliveryStatus.InProgress)
                .Include(d => d.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .ToListAsync();

            // Convert to DTOs to break circular references
            var deliveryDTOs = deliveries.Select(d => new DeliveryDTO
            {
                DeliveryId = d.DeliveryId,
                DeliveryPersonId = d.DeliveryPersonId,
                OrderId = d.OrderId,
                Status = d.Status,
                PickupTime = d.PickupTime,
                EstimatedDeliveryTime = d.EstimatedDeliveryTime,
                DeliveredTime = d.DeliveredTime,
                DeliveryPersonName = d.DeliveryPerson?.Name,
                Order = new OrderDTO
                {
                    OrderId = d.Order.OrderId,
                    CustomerId = d.Order.UserId,
                
                    OrderDate = d.Order.OrderDate,
                    DeliveryAddress = d.Order.DeliveryAddress,
                    TotalAmount = d.Order.TotalAmount,
                    Status = d.Order.Status,
                    PaymentMethod = d.Order.PaymentMethod,
                    OrderItems = d.Order.OrderItems.Select(oi => new OrderItemDTO
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal,
                        ProductName = oi.Product.ProductName,
                    }).ToList()
                }
            }).ToList();

            return deliveryDTOs;
        }

        public async Task<Delivery> CreateDelivery(int orderId)
        {
            try
            {
                var deliveryPerson = await GetAvailableDeliveryPeople();

                var newDelivery = new Delivery
                {
                    DeliveryId = 0,
                    DeliveryPersonId = deliveryPerson.UserId,
                    OrderId = orderId,
                    Status = Delivery.DeliveryStatus.Pending,
                    PickupTime = null,
                    EstimatedDeliveryTime = DateTime.UtcNow.AddDays(1)
                };

                // Add the new delivery to the context
                _context.Deliveries.Add(newDelivery);
                await _context.SaveChangesAsync();

                return newDelivery;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("No delivery drivers"))
            {
                // Handle the specific case of no available drivers by creating an unassigned delivery
                // with a special ID for the delivery person that indicates "unassigned"
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    throw new InvalidOperationException($"Order with ID {orderId} not found");
                }

                // Update the order status to indicate it's waiting for a driver
                order.Status = "AwaitingDriver";

                // Create a delivery record without an assigned driver
                // Using a placeholder DeliveryPersonId of 0 to indicate unassigned
                var unassignedDelivery = new Delivery
                {
                    DeliveryId = 0,
                    DeliveryPersonId = 0, // Special case - no driver assigned yet
                    OrderId = orderId,
                    Status = Delivery.DeliveryStatus.Pending,
                    PickupTime = null,
                    EstimatedDeliveryTime = DateTime.UtcNow.AddDays(2), // Extra day due to no driver
                };

                _context.Deliveries.Add(unassignedDelivery);
                await _context.SaveChangesAsync();

                // Log this situation for administrative review
                Console.WriteLine($"Created unassigned delivery for order {orderId} due to no available drivers");

                return unassignedDelivery;
            }
        }

        public async Task<Delivery> UpdateDelivery(int id, Delivery updatedDelivery)
        {
            try
            {
                if (updatedDelivery == null)
                {
                    throw new ArgumentNullException(nameof(updatedDelivery), "Delivery cannot be null");
                }

                if (id <= 0)
                {
                    throw new ArgumentException("Delivery ID must be greater than zero", nameof(updatedDelivery.DeliveryId));
                }

                // Verify Delivery exists
                var existingDelivery = await _context.Deliveries.FindAsync(id);
                if (existingDelivery == null)
                {
                    throw new InvalidOperationException($"Delivery with ID {id} not found");
                }

                // Update the existing entity's properties
                existingDelivery.DeliveryPersonId = updatedDelivery.DeliveryPersonId;
                existingDelivery.OrderId = updatedDelivery.OrderId;
                existingDelivery.Status = updatedDelivery.Status;
                existingDelivery.EstimatedDeliveryTime = updatedDelivery.EstimatedDeliveryTime;
                existingDelivery.PickupTime = updatedDelivery.PickupTime;

                // Save changes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while updating delivery: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentNullException &&
                                       ex is not ArgumentException &&
                                       ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error updating delivery with ID {id}: {ex.Message}", ex);
            }

            return updatedDelivery;
        }

        public async Task<DeliveryDTO> UpdateDeliveryStatus(int id, Delivery.DeliveryStatus deliveryStatus)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Delivery ID must be greater than zero");
                }

                // Verify Delivery exists with necessary includes for DTO mapping
                var existingDelivery = await _context.Deliveries
                    .Include(d => d.DeliveryPerson)
                    .Include(d => d.Order)
                        .ThenInclude(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(d => d.DeliveryId == id);

                if (existingDelivery == null)
                {
                    throw new InvalidOperationException($"Delivery with ID {id} not found");
                }

                // Update the existing entity's properties
                existingDelivery.Status = deliveryStatus;

                if (deliveryStatus == Delivery.DeliveryStatus.Completed)
                {
                    existingDelivery.DeliveredTime = DateTime.UtcNow;
                    existingDelivery.DeliveryPerson.Status = DeliveryPerson.DeliveryPersonStatus.Available;
                }
                else if (deliveryStatus == Delivery.DeliveryStatus.InProgress)
                {
                    existingDelivery.PickupTime = DateTime.UtcNow;
                    // Also update the corresponding order status
                    var order = await _context.Orders.FindAsync(existingDelivery.OrderId);
                    if (order != null)
                    {
                        order.Status = "InProgress";
                        Console.WriteLine($"Order {order.OrderId} status updated to InProgress");
                    }
                }
                else if (deliveryStatus == Delivery.DeliveryStatus.Cancelled)
                {
                    existingDelivery.PickupTime = null;
                    existingDelivery.EstimatedDeliveryTime = null;
                    existingDelivery.DeliveryPerson.Status = DeliveryPerson.DeliveryPersonStatus.Available;
                }

                // Save changes for all cases (not just Cancelled)
                await _context.SaveChangesAsync();

                // Convert to DTO to break circular references
                var deliveryDTO = new DeliveryDTO
                {
                    DeliveryId = existingDelivery.DeliveryId,
                    DeliveryPersonId = existingDelivery.DeliveryPersonId,
                    OrderId = existingDelivery.OrderId,
                    Status = existingDelivery.Status,
                    PickupTime = existingDelivery.PickupTime,
                    EstimatedDeliveryTime = existingDelivery.EstimatedDeliveryTime,
                    DeliveredTime = existingDelivery.DeliveredTime,
                    DeliveryPersonName = existingDelivery.DeliveryPerson?.Name,
                    Order = existingDelivery.Order != null ? new OrderDTO
                    {
                        OrderId = existingDelivery.Order.OrderId,
                        CustomerId = existingDelivery.Order.UserId,
                        OrderDate = existingDelivery.Order.OrderDate,
                        DeliveryAddress = existingDelivery.Order.DeliveryAddress,
                        TotalAmount = existingDelivery.Order.TotalAmount,
                        Status = existingDelivery.Order.Status,
                        PaymentMethod = existingDelivery.Order.PaymentMethod,
                        OrderItems = existingDelivery.Order.OrderItems?.Select(oi => new OrderItemDTO
                        {
                            OrderItemId = oi.OrderItemId,
                            ProductId = oi.ProductId,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            Subtotal = oi.Subtotal,
                            ProductName = oi.Product?.ProductName
                        }).ToList()
                    } : null
                };

                return deliveryDTO;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while updating delivery: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentNullException &&
                                      ex is not ArgumentException &&
                                      ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error updating delivery with ID {id}: {ex.Message}", ex);
            }
        }


        public async Task<bool> AssignDriversToUnassignedDeliveries()
        {
            try
            {
                // Find all deliveries with DeliveryPersonId = 0 (unassigned)
                var unassignedDeliveries = await _context.Deliveries
                    .Where(d => d.DeliveryPersonId == 0)
                    .ToListAsync();

                if (!unassignedDeliveries.Any())
                {
                    return false; // No unassigned deliveries to process
                }

                bool anyAssigned = false;

                foreach (var delivery in unassignedDeliveries)
                {
                    try
                    {
                        // Try to get an available driver
                        var deliveryPerson = await GetAvailableDeliveryPeople();

                        // Assign the driver to this delivery
                        delivery.DeliveryPersonId = deliveryPerson.UserId;

                        // Update the order status
                        var order = await _context.Orders.FindAsync(delivery.OrderId);
                        if (order != null)
                        {
                            order.Status = "Processing";
                        }

                        anyAssigned = true;
                        await _context.SaveChangesAsync();

                        Console.WriteLine($"Assigned driver {deliveryPerson.Name} to previously unassigned delivery {delivery.DeliveryId}");
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("No delivery drivers"))
                    {
                        // Still no drivers available, will try again next time
                        Console.WriteLine("No drivers available to assign to unassigned deliveries");
                        break; // No point trying the rest if we can't get a driver
                    }
                }

                return anyAssigned;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error assigning drivers to unassigned deliveries: {ex.Message}", ex);
            }
        }

        private async Task<DeliveryPerson> GetAvailableDeliveryPeople()
        {
            try
            {
                var deliveryPersons = await _context.Users
                    .Where(p => p.Role == User.UserRole.DeliveryPerson)  // Only get delivery persons
                    .Where(p => p.Status == DeliveryPerson.DeliveryPersonStatus.Available)
                    .OfType<DeliveryPerson>()  // Filter to DeliveryPerson type
                    .ToListAsync();

                // Check if there are any available delivery persons
                if (deliveryPersons == null || deliveryPersons.Count == 0)
                {
                    throw new InvalidOperationException("No delivery drivers are currently available");
                }

                foreach (var deliveryPerson1 in deliveryPersons)
                {
                    Console.WriteLine($"Delivery person: {deliveryPerson1.Name}, Status: {deliveryPerson1.Status}");
                }

                // Pick random deliveryPerson from list
                var random = new Random();
                var randomIndex = random.Next(deliveryPersons.Count);
                var deliveryPerson = deliveryPersons[randomIndex];
                deliveryPerson.Status = DeliveryPerson.DeliveryPersonStatus.Busy;
                Console.WriteLine($"Delivery person selected: {deliveryPerson.Name}");

                return deliveryPerson;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving available delivery persons: {ex.Message}", ex);
            }
        }
    }
}
