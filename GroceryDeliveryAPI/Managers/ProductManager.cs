using GroceryDeliveryAPI.Models;
using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GroceryDeliveryAPI.Managers
{
    public class ProductManager
    {
        private readonly GroceryDeliveryContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductManager(GroceryDeliveryContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? categoryId = null)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query
                .OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImagePath = p.ImagePath,
                Description = p.Description
            });
        }

        public async Task<int> GetTotalProductCountAsync(string? searchTerm = null, int? categoryId = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return null;

            return new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImagePath = product.ImagePath,
                Description = product.Description
            };
        }

        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO productDto)
        {
            var product = new Product
            {
                ProductName = productDto.ProductName,
                CategoryId = productDto.CategoryId,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                ImagePath = productDto.ImagePath,
                Description = productDto.Description
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImagePath = product.ImagePath,
                Description = product.Description
            };
        }

        public async Task<ProductDTO?> UpdateProductAsync(int id, UpdateProductDTO productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return null;

            product.ProductName = productDto.ProductName;
            product.CategoryId = productDto.CategoryId;
            product.Price = productDto.Price;
            product.StockQuantity = productDto.StockQuantity;
            product.ImagePath = productDto.ImagePath;
            product.Description = productDto.Description;

            await _context.SaveChangesAsync();

            return new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImagePath = product.ImagePath,
                Description = product.Description
            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> SaveProductImageAsync(string base64Image, string fileName)
        {
            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                var imageBytes = Convert.FromBase64String(base64Image);
                await File.WriteAllBytesAsync(filePath, imageBytes);

                return $"/images/products/{uniqueFileName}";
            }
            catch (Exception)
            {
                throw new Exception("Error saving image file");
            }
        }
    }
} 