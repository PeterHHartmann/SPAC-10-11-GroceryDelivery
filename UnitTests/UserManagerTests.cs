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
    public sealed class UserManagerTests
    {
        private DbContextOptions<GroceryDeliveryContext> _options;
        private GroceryDeliveryContext _context;
        private UserManager _userManager;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<GroceryDeliveryContext>()
                .UseInMemoryDatabase(databaseName: $"GroceryDeliveryDB_{Guid.NewGuid()}")
                .Options;

            _context = new GroceryDeliveryContext(_options);
            _userManager = new UserManager(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAllUsersAsync_WhenUsersExist_ReturnsUsersWithoutPasswords()
        {
            // Arrange
            await _context.Users.AddRangeAsync(new[]
            {
                new User { UserId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Password = "hashedpassword1",  PhoneNumber = "1234567890",
                Address = "123 Main St",},
                new User { UserId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Password = "hashedpassword2", PhoneNumber = "1234567890",
                Address = "123 Main St", }
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _userManager.GetAllUsersAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(u => u.Password == null));
        }

        [TestMethod]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUserWithoutPassword()
        {
            // Arrange
            var user = new User {UserId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Password = "hashedpassword", PhoneNumber = "1234567890", Address = "123 Main St" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userManager.GetUserByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John", result.FirstName);
            Assert.IsNull(result.Password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act
            await _userManager.GetUserByIdAsync(1);
        }

        [TestMethod]
        public async Task GetUserByEmailAsync_WhenUserExists_ReturnsUserWithoutPassword()
        {
            // Arrange
            var user = new User { UserId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Password = "hashedpassword", PhoneNumber = "1234567890", Address = "123 Main St",
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userManager.GetUserByEmailAsync("john@example.com");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John", result.FirstName);
            Assert.IsNull(result.Password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetUserByEmailAsync_WhenUserDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act
            await _userManager.GetUserByEmailAsync("nonexistent@example.com");
        }

        [TestMethod]
        public async Task AddUserAsync_WithValidUser_AddsUser()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                Password = "plaintextpassword",
                Role = User.UserRole.Customer
            };

            // Act
            var result = await _userManager.AddUserAsync(user, User.UserRole.Customer);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John", result.FirstName);
            Assert.AreEqual(1, _context.Users.Count());
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("plaintextpassword", result.Password));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddUserAsync_WithNullUser_ThrowsArgumentNullException()
        {
            // Act
            await _userManager.AddUserAsync(null, User.UserRole.Customer);
        }

        [TestMethod]
        public async Task UpdateUserAsync_WithValidUser_UpdatesUser()
        {
            // Arrange
            var user = new User {UserId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Password = "hashedpassword", PhoneNumber = "1234567890", Address = "123 Main St" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var updatedUser = new User { UserId = 1, FirstName = "UpdatedJohn", LastName = "UpdatedDoe", Email = "updatedjohn@example.com", Password = "newpassword" };

            // Act
            await _userManager.UpdateUserAsync(1, updatedUser);

            // Assert
            var result = await _context.Users.FindAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("UpdatedJohn", result.FirstName);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("newpassword", result.Password));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task UpdateUserAsync_WhenUserDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            var updatedUser = new User { UserId = 1, FirstName = "UpdatedJohn", LastName = "UpdatedDoe", Email = "updatedjohn@example.com", Password = "newpassword" };

            // Act
            await _userManager.UpdateUserAsync(1, updatedUser);
        }

        [TestMethod]
        public async Task DeleteUserAsync_WithExistingUser_DeletesUser()
        {
            // Arrange
            var user = new User { UserId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Password = "hashedpassword", Address="123 Main St.", PhoneNumber = "123456789" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            await _userManager.DeleteUserAsync(1);

            // Assert
            Assert.AreEqual(0, _context.Users.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task DeleteUserAsync_WhenUserDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act
            await _userManager.DeleteUserAsync(1);
        }

        [TestMethod]
        public void HashPassword_WithValidPassword_HashesPassword()
        {
            // Arrange
            var user = new User { Password = "plaintextpassword" };

            // Act
            _userManager.HashPassword(user);

            // Assert
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("plaintextpassword", user.Password));
        }

        [TestMethod]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var user = new User { Password = BCrypt.Net.BCrypt.HashPassword("plaintextpassword") };

            // Act
            var result = _userManager.VerifyPassword(user, "plaintextpassword");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var user = new User { Password = BCrypt.Net.BCrypt.HashPassword("plaintextpassword") };

            // Act
            var result = _userManager.VerifyPassword(user, "wrongpassword");

            // Assert
            Assert.IsFalse(result);
        }
    }
}