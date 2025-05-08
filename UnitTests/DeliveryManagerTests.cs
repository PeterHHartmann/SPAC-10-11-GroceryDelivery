using GroceryDeliveryAPI.Context;
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
    public class DeliveryManagerTests
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
        public async Task GetAllDeliveries_ReturnsAllDeliveries()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();
            var delivery = await CreateTestDelivery(deliveryPerson.UserId, order.OrderId);

            // Act
            var result = await _deliveryManager.GetAllDeliveries();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(delivery.DeliveryId, result[0].DeliveryId);
        }

        [TestMethod]
        public async Task GetDeliveryById_ExistingDelivery_ReturnsDelivery()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();
            var delivery = await CreateTestDelivery(deliveryPerson.UserId, order.OrderId);

            // Act
            var result = await _deliveryManager.GetDeliveryById(delivery.DeliveryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(delivery.DeliveryId, result.DeliveryId);
        }

        [TestMethod]
        public async Task GetDeliveryById_NonexistentDelivery_ReturnsNull()
        {
            // Act
            var result = await _deliveryManager.GetDeliveryById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetDeliveriesByDeliveryPersonId_ReturnsActiveDeliveries()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();
            var delivery = await CreateTestDelivery(deliveryPerson.UserId, order.OrderId, Delivery.DeliveryStatus.Pending);

            // Act
            var result = await _deliveryManager.GetDeliveriesByDeliveryPersonId(deliveryPerson.UserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(delivery.DeliveryId, result[0].DeliveryId);
        }

        [TestMethod]
        public async Task CreateDelivery_WithAvailableDriver_CreatesDeliveryWithDriver()
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
            Assert.IsNotNull(await _context.Deliveries.FindAsync(result.DeliveryId));
        }

        [TestMethod]
        public async Task CreateDelivery_WithoutAvailableDriver_CreatesUnassignedDelivery()
        {
            // Arrange
            var order = await CreateTestOrder();

            // Act
            var result = await _deliveryManager.CreateDelivery(order.OrderId);

            // Assert
            Assert.IsNotNull(result);
            var systemDriver = await _context.Users
                .FirstOrDefaultAsync(u => u.FirstName == "System" && u.LastName == "Unassigned");
            Assert.IsNotNull(systemDriver);
            Assert.AreEqual(systemDriver.UserId, result.DeliveryPersonId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateDelivery_NullDelivery_ThrowsArgumentNullException()
        {
            await _deliveryManager.UpdateDelivery(1, null);
        }

        [TestMethod]
        public async Task UpdateDeliveryStatus_ToCompleted_UpdatesStatusAndDeliveryTime()
        {
            // Arrange
            var deliveryPerson = await CreateTestDeliveryPerson();
            var order = await CreateTestOrder();
            var delivery = await CreateTestDelivery(deliveryPerson.UserId, order.OrderId);

            // Act
            var result = await _deliveryManager.UpdateDeliveryStatus(delivery.DeliveryId, Delivery.DeliveryStatus.Completed);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Delivery.DeliveryStatus.Completed, result.Status);
            Assert.IsNotNull(result.DeliveredTime);
        }

        [TestMethod]
        public async Task AssignDriversToUnassignedDeliveries_WithAvailableDrivers_AssignsDeliveriesSuccessfully()
        {
            // Arrange
            var order = await CreateTestOrder();
            var systemDelivery = await _deliveryManager.CreateDelivery(order.OrderId); // Creates unassigned delivery
            var availableDriver = await CreateTestDeliveryPerson();

            // Act
            var result = await _deliveryManager.AssignDriversToUnassignedDeliveries();

            // Assert
            Assert.IsTrue(result);
            var updatedDelivery = await _context.Deliveries.FindAsync(systemDelivery.DeliveryId);
            Assert.AreEqual(availableDriver.UserId, updatedDelivery.DeliveryPersonId);
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

        private async Task<Delivery> CreateTestDelivery(int deliveryPersonId, int orderId, Delivery.DeliveryStatus status = Delivery.DeliveryStatus.Pending)
        {
            var delivery = new Delivery
            {
                DeliveryPersonId = deliveryPersonId,
                OrderId = orderId,
                Status = status,
                EstimatedDeliveryTime = DateTime.UtcNow.AddHours(1)
            };

            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();
            return delivery;
        }
    }
}