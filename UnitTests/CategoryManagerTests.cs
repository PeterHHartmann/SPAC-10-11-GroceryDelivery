using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.DTO_s;
using GroceryDeliveryAPI.Managers;
using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public sealed class CategoryManagerTests
    {
        private DbContextOptions<GroceryDeliveryContext> _options;
        private GroceryDeliveryContext _context;
        private CategoryManager _categoryManager;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<GroceryDeliveryContext>()
                .UseInMemoryDatabase(databaseName: $"GroceryDeliveryDB_{Guid.NewGuid()}")
                .Options;

            _context = new GroceryDeliveryContext(_options);
            _categoryManager = new CategoryManager(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetCategoriesAsync_WhenCategoriesExist_ReturnsOrderedCategories()
        {
            // Arrange
            await _context.Categories.AddRangeAsync(new[]
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryManager.GetCategoriesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Category1", result.First().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetCategoriesAsync_WhenNoCategoriesExist_ThrowsKeyNotFoundException()
        {
            // Act
            await _categoryManager.GetCategoriesAsync();
        }

        [TestMethod]
        public async Task GetCategoryByIdAsync_WhenCategoryExists_ReturnsCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "TestCategory" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryManager.GetCategoryByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("TestCategory", result.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetCategoryByIdAsync_WhenCategoryDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Act
            await _categoryManager.GetCategoryByIdAsync(1);
        }

        [TestMethod]
        public async Task GetCategoryByNameAsync_WhenCategoryExists_ReturnsCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "TestCategory" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryManager.GetCategoryByNameAsync("TestCategory");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TestCategory", result.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetCategoryByNameAsync_WhenCategoryDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Act
            await _categoryManager.GetCategoryByNameAsync("NonExistentCategory");
        }

        [TestMethod]
        public async Task GetCategoryByIdWithProductsAsync_WhenCategoryExists_ReturnsCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "TestCategory" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryManager.GetCategoryByIdWithProductsAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TestCategory", result.Name);
        }

        [TestMethod]
        public async Task CreateCategoryAsync_WithValidCategory_CreatesNewCategory()
        {
            // Arrange
            var categoryDto = new CategoryDTO { Name = "NewCategory" };

            // Act
            var result = await _categoryManager.CreateCategoryAsync(categoryDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("NewCategory", result.Name);
            Assert.AreEqual(1, _context.Categories.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CreateCategoryAsync_WithNullCategory_ThrowsArgumentNullException()
        {
            // Act
            await _categoryManager.CreateCategoryAsync(null);
        }

        [TestMethod]
        public async Task UpdateCategoryAsync_WithValidCategory_UpdatesExistingCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "OldName" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var updateDto = new CategoryDTO { Id = 1, Name = "UpdatedName" };

            // Act
            var result = await _categoryManager.UpdateCategoryAsync(1, updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UpdatedName", result.Name);
            Assert.AreEqual("UpdatedName", (await _context.Categories.FindAsync(1)).Name);
        }

        [TestMethod]
        public async Task DeleteCategoryAsync_WithExistingCategory_DeletesCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "ToDelete" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            await _categoryManager.DeleteCategoryAsync(1);

            // Assert
            Assert.AreEqual(0, _context.Categories.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteCategoryAsync_WithNonExistentCategory_ThrowsKeyNotFoundException()
        {
            // Act
            await _categoryManager.DeleteCategoryAsync(1);
        }
    }
}
