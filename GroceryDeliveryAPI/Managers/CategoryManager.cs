using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.DTO_s;
using GroceryDeliveryAPI.DTOs;
using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryDeliveryAPI.Managers
{
    public class CategoryManager
    {
        private readonly GroceryDeliveryContext _context;
        public CategoryManager(GroceryDeliveryContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            if (categories == null || !categories.Any())
            {
                throw new KeyNotFoundException("No categories found.");
            }
            var categoryDTOs = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            }).ToList().OrderBy(c => c.Id);
            return categoryDTOs;
        }
        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
            return categoryDTO;
        }

        public async Task<CategoryDTO> GetCategoryByNameAsync(string name)
        {
            var category = await _context.Categories
               .Where(c => c.Name.ToLower() == name.ToLower())
               .FirstOrDefaultAsync();
           
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with name {name} not found.");
            }
            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
            return categoryDTO;
        }

        public async Task<CategoryWithProductsDTO> GetCategoryByIdWithProductsAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            var categoryDTO = new CategoryWithProductsDTO
            {
                Id = category.Id,
                Name = category.Name,
                Products = category.Products.Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImagePath = p.ImagePath,
                    Description = p.Description
                }).ToList()
            };

            return categoryDTO;
        }
        public async Task<Category> CreateCategoryAsync(CategoryDTO category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Category name cannot be null or empty.", nameof(category));
            }

            var existingCategory = await _context.Categories
                .Where(c => c.Name.ToLower() == category.Name.ToLower())
                .FirstOrDefaultAsync();

            if (existingCategory != null)
            {
                return existingCategory;
            }

            int newCategoryId = 1;
            if (await _context.Products.AnyAsync())
            {
                newCategoryId = await _context.Categories.MaxAsync(c => c.Id) + 1;
            }

            Category newCategory = new Category
            {
                Name = category.Name,
                Id = newCategoryId
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return newCategory;
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryDTO category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            existingCategory.Name = category.Name;
            await _context.SaveChangesAsync();
            var updatedCategory = new CategoryDTO
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name
            };
            return updatedCategory;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }


    }
}
