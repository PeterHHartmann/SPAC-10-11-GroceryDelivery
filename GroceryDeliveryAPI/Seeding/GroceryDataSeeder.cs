using CsvHelper;
using CsvHelper.Configuration;
using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GroceryDeliveryAPI.Seeding
{
    /// <summary>
    /// Responsible for seeding the grocery database with initial product and category data from CSV files
    /// </summary>
    public class GroceryDataSeeder
    {
        private readonly GroceryDeliveryContext _context;

        public GroceryDataSeeder(GroceryDeliveryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Main entry point for importing data from CSV into the database
        /// Only imports data if the database is empty
        /// </summary>
        /// <param name="csvFilePath">Path to the CSV file containing grocery data</param>
        public async Task ImportDataAsync(string csvFilePath)
        {
            await EnsureDatabaseCreatedAsync();

            if (!await IsDatabaseEmptyAsync())
            {
                Console.WriteLine("Database already contains data. Skipping import.");
                return;
            }

            await ImportGroceriesFromCsvAsync(csvFilePath);
        }

        private async Task EnsureDatabaseCreatedAsync()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create/migrate database", ex);
            }
        }

        /// <summary>
        /// Orchestrates the CSV import process including validation, reading and processing records
        /// </summary>
        private async Task ImportGroceriesFromCsvAsync(string csvFilePath)
        {
            ValidateFilePath(csvFilePath);

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                BadDataFound = null
            };

            try
            {
                var records = await ReadCsvRecordsAsync(csvFilePath, configuration);
                await ProcessGroceryRecordsAsync(records, csvFilePath);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Failed to import groceries from CSV: {ex.Message}", ex);
            }
        }

        private async Task<List<GroceryRecord>> ReadCsvRecordsAsync(string csvFilePath, CsvConfiguration configuration)
        {
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, configuration);
            try
            {
                return csv.GetRecords<GroceryRecord>().ToList();
            }
            catch (CsvHelperException csvEx)
            {
                throw new InvalidOperationException($"Error reading CSV file: {csvEx.Message}", csvEx);
            }
        }

        /// <summary>
        /// Processes each record from the CSV by validating data, ensuring categories exist,
        /// and adding products to the database
        /// </summary>
        private async Task ProcessGroceryRecordsAsync(List<GroceryRecord> records, string csvFilePath)
        {
            foreach (var record in records)
            {
                try
                {
                    ValidateGroceryRecord(record);
                    var category = await EnsureCategoryExistsAsync(record);
                    await AddProductIfNotExistsAsync(record, category, csvFilePath);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Error processing record - ProductId: {record.product_id}, " +
                        $"ProductName: {record.product_name}, " +
                        $"CategoryId: {record.category_id}, " +
                        $"CategoryName: {record.category_name}", ex);
                }
            }
        }


        /// <summary>
        /// Validates grocery record fields for basic data integrity
        /// </summary>
        private void ValidateGroceryRecord(GroceryRecord record)
        {
            if (string.IsNullOrEmpty(record.product_name))
                throw new InvalidDataException($"Product name is required for product ID: {record.product_id}");

            if (record.price < 0)
                throw new InvalidDataException($"Price cannot be negative for product: {record.product_name}");

            if (record.stock_quantity < 0)
                throw new InvalidDataException($"Stock quantity cannot be negative for product: {record.product_name}");
        }

        /// <summary>
        /// Finds or creates a category based on the record information
        /// </summary>
        private async Task<Category> EnsureCategoryExistsAsync(GroceryRecord record)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == record.category_id);

            if (category == null)
            {
                category = new Category
                {
                    Id = record.category_id,
                    Name = record.category_name
                };
                _context.Categories.Add(category);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    throw new InvalidOperationException(
                        $"Failed to save category {record.category_name}. Database error: {dbEx.Message}", dbEx);
                }
            }

            return category;
        }

        /// <summary>
        /// Adds a new product to the database if it doesn't already exist
        /// </summary>
        private async Task AddProductIfNotExistsAsync(GroceryRecord record, Category category, string csvFilePath)
        {
            if (await _context.Products.AnyAsync(p => p.ProductId == record.product_id))
                return;

            var description = await ReadProductDescriptionAsync(record, csvFilePath);
            var product = CreateProductFromRecord(record, category.Id, description);

            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new InvalidOperationException(
                    $"Failed to save product {record.product_name}. Database error: {dbEx.Message}", dbEx);
            }
        }


        /// <summary>
        /// Reads product description from an external file specified in the record
        /// Assumes description files are located relative to the CSV file
        /// </summary>
        private async Task<string> ReadProductDescriptionAsync(GroceryRecord record, string csvFilePath)
        {
            try
            {
                var descriptionPath = Path.Combine(
                    Path.GetDirectoryName(csvFilePath)!,
                    record.description_path.TrimStart('/'));

                if (!File.Exists(descriptionPath))
                    throw new FileNotFoundException($"Description file not found: {descriptionPath}");

                return await File.ReadAllTextAsync(descriptionPath);
            }
            catch (Exception ioEx) when (ioEx is IOException || ioEx is FileNotFoundException)
            {
                throw new InvalidOperationException(
                    $"Failed to read description file for product {record.product_name}: {ioEx.Message}", ioEx);
            }
        }

        private Product CreateProductFromRecord(GroceryRecord record, int categoryId, string description)
        {
            return new Product
            {
                ProductId = record.product_id,
                ProductName = record.product_name,
                CategoryId = categoryId,
                Price = record.price,
                StockQuantity = record.stock_quantity,
                ImagePath = record.image_path,
                Description = description
            };
        }

        private void ValidateFilePath(string csvFilePath)
        {
            if (string.IsNullOrEmpty(csvFilePath))
                throw new ArgumentNullException(nameof(csvFilePath), "CSV file path cannot be null or empty.");

            if (!File.Exists(csvFilePath))
                throw new FileNotFoundException($"CSV file not found at path: {csvFilePath}");
        }


        /// <summary>
        /// Checks if the database is empty (contains no categories or products)
        /// </summary>
        private async Task<bool> IsDatabaseEmptyAsync()
        {
            return !await _context.Categories.AnyAsync() && !await _context.Products.AnyAsync();
        }
    }
}
