using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.DTO_s;
using GroceryDeliveryAPI.Managers;
using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class DeliveryDriverManagerTests
    {
        private DbContextOptions<GroceryDeliveryContext> _options;
        private GroceryDeliveryContext _context;
        private DeliveryManager _deliveryManager;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<GroceryDeliveryContext>()
                .UseInMemoryDatabase(databaseName: $"GroceryDeliveryDB_{Guid.NewGuid()}")
                .Options;

            _context = new GroceryDeliveryContext(_options);
            _deliveryManager = new DeliveryManager(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAllDeliveries_WhenDeliveriesExist_ReturnsAllDeliveries()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();

            var delivery = new Delivery
            {
                DeliveryPersonId = deliveryPerson.UserId,
                OrderId = order.OrderId,
                Status = Delivery.DeliveryStatus.Pending,
                EstimatedDeliveryTime = DateTime.UtcNow.AddHours(1)
            };

            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();

            // Act
            var result = await _deliveryManager.GetAllDeliveries();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(delivery.OrderId, result[0].OrderId);
        }

        [TestMethod]
        public async Task GetDeliveryById_WhenDeliveryExists_ReturnsDelivery()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();

            var delivery = new Delivery
            {
                DeliveryPersonId = deliveryPerson.UserId,
                OrderId = order.OrderId,
                Status = Delivery.DeliveryStatus.Pending,
                EstimatedDeliveryTime = DateTime.UtcNow.AddHours(1)
            };

            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();

            // Act
            var result = await _deliveryManager.GetDeliveryById(delivery.DeliveryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(delivery.OrderId, result.OrderId);
        }

        [TestMethod]
        public async Task GetDeliveriesByDeliveryPersonId_WhenDeliveriesExist_ReturnsDeliveries()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();

            var delivery = new Delivery
            {
                DeliveryPersonId = deliveryPerson.UserId,
                OrderId = order.OrderId,
                Status = Delivery.DeliveryStatus.Pending,
                EstimatedDeliveryTime = DateTime.UtcNow.AddHours(1)
            };

            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();

            // Act
            var result = await _deliveryManager.GetDeliveriesByDeliveryPersonId(deliveryPerson.UserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(delivery.OrderId, result[0].OrderId);
        }

        [TestMethod]
        public async Task CreateDelivery_WithValidOrder_CreatesDelivery()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();

            // Act
            var result = await _deliveryManager.CreateDelivery(order.OrderId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(order.OrderId, result.OrderId);
            Assert.AreEqual(Delivery.DeliveryStatus.Pending, result.Status);
        }

        [TestMethod]
        public async Task CreateDelivery_WithNoAvailableDrivers_CreatesUnassignedDelivery()
        {
            // Arrange
            var order = await CreateTestOrder();
            // Don't create any delivery persons - this will force the unassigned case

            // Act
            var result = await _deliveryManager.CreateDelivery(order.OrderId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(order.OrderId, result.OrderId);
            Assert.AreEqual(Delivery.DeliveryStatus.Pending, result.Status);

            var systemDriver = await _context.Users
                .FirstOrDefaultAsync(u => u.FirstName == "System" && u.LastName == "Unassigned");
            Assert.IsNotNull(systemDriver);
            Assert.AreEqual(systemDriver.UserId, result.DeliveryPersonId);
        }

        [TestMethod]
        public async Task UpdateDelivery_WithValidDelivery_UpdatesDelivery()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();

            var delivery = new Delivery
            {
                DeliveryPersonId = deliveryPerson.UserId,
                OrderId = order.OrderId,
                Status = Delivery.DeliveryStatus.Pending,
                EstimatedDeliveryTime = DateTime.UtcNow.AddHours(1)
            };

            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();

            var updatedDelivery = new UpdateDeliveryDTO
            {
                DeliveryId = delivery.DeliveryId,
                DeliveryPersonId = deliveryPerson.UserId,
                OrderId = order.OrderId,
                Status = Delivery.DeliveryStatus.InProgress,
                PickupTime = DateTime.UtcNow,
                EstimatedDeliveryTime = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var result = await _deliveryManager.UpdateDelivery(delivery.DeliveryId, updatedDelivery);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Delivery.DeliveryStatus.InProgress, result.Status);
        }

        [TestMethod]
        public async Task UpdateDeliveryStatus_ToInProgress_UpdatesStatusAndTimes()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();

            var delivery = new Delivery
            {
                DeliveryPersonId = deliveryPerson.UserId,
                OrderId = order.OrderId,
                Status = Delivery.DeliveryStatus.Pending,
                EstimatedDeliveryTime = DateTime.UtcNow.AddHours(1)
            };

            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();

            // Act
            var result = await _deliveryManager.UpdateDeliveryStatus(delivery.DeliveryId, Delivery.DeliveryStatus.InProgress);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Delivery.DeliveryStatus.InProgress, result.Status);
            Assert.IsNotNull(result.PickupTime);
        }

        [TestMethod]
        public async Task AssignDriversToUnassignedDeliveries_WithAvailableDrivers_AssignsDeliveries()
        {
            // Arrange
            var order = await CreateTestOrder();
            var delivery = await _deliveryManager.CreateDelivery(order.OrderId); // This will create an unassigned delivery
            var deliveryPerson = await CreateTestDeliveryPerson(); // Create an available driver

            // Act
            var result = await _deliveryManager.AssignDriversToUnassignedDeliveries();

            // Assert
            Assert.IsTrue(result);
            var updatedDelivery = await _context.Deliveries.FindAsync(delivery.DeliveryId);
            Assert.AreNotEqual("System", (await _context.Users.FindAsync(updatedDelivery.DeliveryPersonId)).FirstName);
        }

        private async Task<DeliveryPerson> CreateTestDeliveryPerson()
        {
            var deliveryPerson = new DeliveryPerson
            {
                FirstName = "Test",
                LastName = "Driver",
                Email = "testdriver@example.com",
                Password = "password",
                PhoneNumber = "1234567890",
                Address = "123 Test St",
                Role = User.UserRole.DeliveryPerson,
                Status = DeliveryPerson.DeliveryPersonStatus.Available,
                RegistrationDate = DateTime.UtcNow
            };

            await _context.Users.AddAsync(deliveryPerson);
            await _context.SaveChangesAsync();
            return deliveryPerson;
        }

        private async Task<Order> CreateTestOrder()
        {
            var user = new User
            {
                FirstName = "Test",
                LastName = "Customer",
                Email = "testcustomer@example.com",
                Password = "password",
                PhoneNumber = "1234567890",
                Address = "123 Test St",
                Role = User.UserRole.Customer,
                RegistrationDate = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var order = new Order
            {
                UserId = user.UserId,
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = "123 Test St",
                TotalAmount = 100.00m,
                Status = "Pending",
                PaymentMethod = "Credit Card"
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateDeliveryStatus_WithInvalidId_ThrowsArgumentException()
        {
            // Act
            await _deliveryManager.UpdateDeliveryStatus(-1, Delivery.DeliveryStatus.InProgress);
        }

        [TestMethod]
        public async Task UpdateDeliveryStatus_ToCompleted_UpdatesDeliveryPersonStatus()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();
            var delivery = new Delivery
            {
                DeliveryPersonId = deliveryPerson.UserId,
                OrderId = order.OrderId,
                Status = Delivery.DeliveryStatus.InProgress,
                PickupTime = DateTime.UtcNow,
                EstimatedDeliveryTime = DateTime.UtcNow.AddHours(1)
            };
            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();

            // Act
            var result = await _deliveryManager.UpdateDeliveryStatus(delivery.DeliveryId, Delivery.DeliveryStatus.Completed);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Delivery.DeliveryStatus.Completed, result.Status);
            Assert.IsNotNull(result.DeliveredTime);
            var updatedDeliveryPerson = await _context.Users.FindAsync(deliveryPerson.UserId);
            Assert.AreEqual(DeliveryPerson.DeliveryPersonStatus.Available, updatedDeliveryPerson.Status);
        }

        [TestMethod]
        public async Task UpdateDeliveryStatus_ToCancelled_ResetsDeliveryPersonStatus()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();
            var delivery = new Delivery
            {
                DeliveryPersonId = deliveryPerson.UserId,
                OrderId = order.OrderId,
                Status = Delivery.DeliveryStatus.InProgress,
                PickupTime = DateTime.UtcNow,
                EstimatedDeliveryTime = DateTime.UtcNow.AddHours(1)
            };
            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();

            // Act
            var result = await _deliveryManager.UpdateDeliveryStatus(delivery.DeliveryId, Delivery.DeliveryStatus.Cancelled);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Delivery.DeliveryStatus.Cancelled, result.Status);
            Assert.IsNull(result.PickupTime);
            Assert.IsNull(result.EstimatedDeliveryTime);
            var updatedDeliveryPerson = await _context.Users.FindAsync(deliveryPerson.UserId);
            Assert.AreEqual(DeliveryPerson.DeliveryPersonStatus.Available, updatedDeliveryPerson.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateDelivery_WithNullDelivery_ThrowsArgumentNullException()
        {
            // Act
            await _deliveryManager.UpdateDelivery(1, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateDelivery_WithInvalidId_ThrowsArgumentException()
        {
            // Arrange
            var delivery = new UpdateDeliveryDTO { DeliveryId = 1 };

            // Act
            await _deliveryManager.UpdateDelivery(-1, delivery);
        }

        [TestMethod]
        public async Task GetDeliveriesByDeliveryPersonId_WithNoDeliveries_ReturnsEmptyList()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();

            // Act
            var result = await _deliveryManager.GetDeliveriesByDeliveryPersonId(deliveryPerson.UserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetDeliveryById_WithNonExistentId_ReturnsNull()
        {
            // Act
            var result = await _deliveryManager.GetDeliveryById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AssignDriversToUnassignedDeliveries_WithNoSystemDeliveryPerson_ReturnsFalse()
        {
            // Act
            var result = await _deliveryManager.AssignDriversToUnassignedDeliveries();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AssignDriversToUnassignedDeliveries_WithNoUnassignedDeliveries_ReturnsFalse()
        {
            // Arrange
            var systemDeliveryPerson = new DeliveryPerson
            {
                FirstName = "System",
                LastName = "Unassigned",
                Email = "system.unassigned@example.com",
                Password = "password",
                PhoneNumber = "000-000-0000",
                Address = "System Address",
                Role = User.UserRole.DeliveryPerson,
                Status = DeliveryPerson.DeliveryPersonStatus.Offline,
                RegistrationDate = DateTime.UtcNow
            };
            await _context.Users.AddAsync(systemDeliveryPerson);
            await _context.SaveChangesAsync();

            // Act
            var result = await _deliveryManager.AssignDriversToUnassignedDeliveries();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetAllDeliveries_WithNoDeliveries_ReturnsEmptyList()
        {
            // Act
            var result = await _deliveryManager.GetAllDeliveries();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
