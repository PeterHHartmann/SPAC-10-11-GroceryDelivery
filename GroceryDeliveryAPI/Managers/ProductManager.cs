using GroceryDeliveryAPI.Models;
using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using GroceryDeliveryAPI.DTO_s;

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

            var productDTOs = new List<ProductDTO>();

            foreach (var p in products)
            {
                var productDTO = new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryId = p.CategoryId,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Description = p.Description
                };

                // Process the image path
                if (!string.IsNullOrEmpty(p.ImagePath))
                {
                    try
                    {
                        // Construct the full path from the relative path in the CSV
                        string fullImagePath = Path.Combine(_environment.ContentRootPath, "Seeding", "GroceryStoreDataset", "dataset", p.ImagePath.TrimStart('/'));

                        if (File.Exists(fullImagePath))
                        {
                            // Read the image file and convert to base64
                            byte[] imageBytes = await File.ReadAllBytesAsync(fullImagePath);
                            string base64Image = Convert.ToBase64String(imageBytes);

                            // Update the ImagePath to include the base64 data with proper MIME type
                            string fileExtension = Path.GetExtension(fullImagePath).ToLowerInvariant();
                            string mimeType = GetMimeTypeFromExtension(fileExtension);

                            productDTO.ImagePath = $"data:{mimeType};base64,{base64Image}";
                        }
                        else
                        {
                            // If image not found, keep the original path
                            productDTO.ImagePath = p.ImagePath;
                            Console.WriteLine($"Image not found: {fullImagePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error and keep original path
                        Console.WriteLine($"Error processing image for product {p.ProductId}: {ex.Message}");
                        productDTO.ImagePath = p.ImagePath;
                    }
                }

                productDTOs.Add(productDTO);
            }

            return productDTOs;
        }

        // Helper method to determine MIME type from file extension
        private string GetMimeTypeFromExtension(string extension)
        {
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream" // Default MIME type
            };
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
            // ID generation is not working, so this is a workaround
            int newProductId = 1;
            if (await _context.Products.AnyAsync())
            {
                newProductId = await _context.Products.MaxAsync(p => p.ProductId) + 1;
            }

            var product = new Product
            {
                ProductId = newProductId, // ID will be generated by the database, but this is a workaround
                ProductName = productDto.ProductName,
                CategoryId = productDto.CategoryId,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                ImagePath = productDto.ImagePath,
                Description = productDto.Description
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Product created: {product}");

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