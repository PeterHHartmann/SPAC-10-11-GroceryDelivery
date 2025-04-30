using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;
using GroceryDeliveryAPI.Models;
using GroceryDeliveryAPI.Context;

public class Seeder
{
    private readonly GroceryDeliveryContext _context;
    private readonly ILogger<Seeder> _logger;
    private const int ProductCsvColumnCount = 8;
    private const int CategoryCsvColumnCount = 4;
    private const int BatchSize = 100;

    // CSV Column Indices (adjust if your CSV structure changes)
    private const int ColProductId = 0;
    private const int ColProductName = 1;
    private const int ColCategoryId = 2;
    private const int ColCategoryName = 3; // Used in ImportCategoriesAsync
    private const int ColPrice = 4;
    private const int ColStockQuantity = 5;
    private const int ColImagePath = 6;
    private const int ColDescriptionPath = 7;

    public Seeder(GroceryDeliveryContext context, ILogger<Seeder> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PrintFileContentAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            _logger.LogWarning("PrintFileContentAsync called with null or empty file path.");
            Console.WriteLine("Error: File path cannot be empty.");
            return;
        }

        try
        {
            _logger.LogInformation($"Attempting to read content from file: {filePath}");

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File not found: {filePath}");
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            Console.WriteLine($"--- File Content: {Path.GetFileName(filePath)} ---");
            int lineNumber = 0;
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    Console.WriteLine($"L{lineNumber}: {line}");
                }
            }
            Console.WriteLine($"--- End of File ({lineNumber} lines) ---");
            _logger.LogInformation($"Successfully printed {lineNumber} lines from {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error reading file: {filePath}");
            Console.WriteLine($"Error reading file: {filePath} - {ex.Message}");
            // Decide if re-throwing is necessary based on application flow
            // throw;
        }
    }

    public async Task SeedDataAsync(string csvFilePath)
    {
        if (string.IsNullOrWhiteSpace(csvFilePath))
        {
            _logger.LogError("Seeding failed. CSV file path is null or empty.");
            return;
        }

        _logger.LogInformation("Starting database seeding process...");

        if (!File.Exists(csvFilePath))
        {
            _logger.LogError($"Seeding failed. CSV file not found: {csvFilePath}");
            return;
        }

        try
        {
            string baseDirectory = Path.GetDirectoryName(Path.GetFullPath(csvFilePath));
            _logger.LogInformation($"Base directory for relative paths: {baseDirectory}");

            await _context.Database.EnsureCreatedAsync();

            bool dataExists = await _context.Categories.AnyAsync() || await _context.Products.AnyAsync();
            if (dataExists)
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Database is empty. Proceeding with seeding.");
            await ImportCategoriesAsync(csvFilePath);
            await ImportProductsAsync(csvFilePath, baseDirectory);

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the database seeding process.");
            throw; // Re-throw to indicate failure to the caller
        }
    }

    private async Task ImportCategoriesAsync(string csvFilePath)
    {
        _logger.LogInformation("Importing categories from {CsvFilePath}", csvFilePath);
        var uniqueCategories = new Dictionary<int, Category>();
        int lineNum = 0;

        using var reader = new StreamReader(csvFilePath);
        string line;
        await reader.ReadLineAsync(); // Skip header row

        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNum++;
            var values = line.Split(',');

            if (values.Length < CategoryCsvColumnCount)
            {
                _logger.LogWarning("Skipping category on line {LineNumber}. Insufficient columns ({ColumnCount})", lineNum + 1, values.Length);
                continue;
            }

            string rawCatId = values[ColCategoryId]?.Trim();
            string catName = values[ColCategoryName]?.Trim();

            if (int.TryParse(rawCatId, out int categoryId) && !string.IsNullOrEmpty(catName))
            {
                if (!uniqueCategories.ContainsKey(categoryId))
                {
                    uniqueCategories.Add(categoryId, new Category { Id = categoryId, Name = catName });
                }
                // else: Category ID already added, skip duplicate. Log if needed.
            }
            else
            {
                _logger.LogWarning("Skipping category on line {LineNumber}. Invalid category ID ('{CategoryId}') or name ('{CategoryName}')", lineNum + 1, rawCatId, catName);
            }
        }

        if (uniqueCategories.Any())
        {
            await _context.Categories.AddRangeAsync(uniqueCategories.Values);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Imported {CategoryCount} unique categories.", uniqueCategories.Count);
        }
        else
        {
            _logger.LogWarning("No valid categories found in the CSV file to import.");
        }
    }

    private async Task ImportProductsAsync(string csvFilePath, string baseDirectory)
    {
        _logger.LogInformation("Importing products from {CsvFilePath}", csvFilePath);
        var products = new List<Product>();
        int lineNum = 0;
        var existingCategoryIds = new HashSet<int>(await _context.Categories.Select(c => c.Id).ToListAsync());

        using var reader = new StreamReader(csvFilePath);
        string line;
        await reader.ReadLineAsync(); // Skip header row

        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNum++;
            var product = await ParseProductFromCsvLineAsync(line, lineNum + 1, existingCategoryIds, baseDirectory);
            if (product != null)
            {
                products.Add(product);
            }
            // Logging for skipped/invalid lines is handled within ParseProductFromCsvLineAsync
        }

        await AddProductsToDatabaseAsync(products);
    }

    private async Task<Product> ParseProductFromCsvLineAsync(string line, int lineNumber, HashSet<int> existingCategoryIds, string baseDirectory)
    {
        var values = line.Split(',');
        if (values.Length < ProductCsvColumnCount)
        {
            _logger.LogWarning("Skipping product on line {LineNumber}. Insufficient columns ({ColumnCount})", lineNumber, values.Length);
            return null;
        }

        // Trim all values at once
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = values[i]?.Trim();
        }

        // Validate and parse data
        bool isValidProductId = int.TryParse(values[ColProductId], out int productId);
        string productName = values[ColProductName];
        bool isValidCategoryId = int.TryParse(values[ColCategoryId], out int categoryId);
        bool isValidPrice = decimal.TryParse(values[ColPrice], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price);
        bool isValidStock = int.TryParse(values[ColStockQuantity], out int stockQuantity);
        string imagePath = values[ColImagePath];
        string descriptionRelativePath = values[ColDescriptionPath];

        // Basic validation checks
        if (!isValidProductId || string.IsNullOrEmpty(productName) || !isValidCategoryId ||
            !isValidPrice || !isValidStock || string.IsNullOrEmpty(imagePath) ||
            string.IsNullOrEmpty(descriptionRelativePath))
        {
            _logger.LogWarning("Skipping product on line {LineNumber}. Invalid data format. Raw Data: ID='{ProductId}', Name='{ProductName}', CatID='{CategoryId}', Price='{Price}', Stock='{Stock}', Img='{ImagePath}', DescPath='{DescriptionPath}'",
                lineNumber, values[ColProductId], productName, values[ColCategoryId], values[ColPrice], values[ColStockQuantity], imagePath, descriptionRelativePath);
            return null;
        }

        // Validate CategoryId existence
        if (!existingCategoryIds.Contains(categoryId))
        {
            _logger.LogWarning("Skipping product '{ProductName}' (ID: {ProductId}) on line {LineNumber}. Category ID '{CategoryId}' not found.",
                productName, productId, lineNumber, categoryId);
            return null;
        }

        // Read description content
        string descriptionContent = await ReadProductDescriptionAsync(productId, descriptionRelativePath, baseDirectory);

        return new Product
        {
            ProductId = productId,
            ProductName = productName,
            CategoryId = categoryId,
            Price = price,
            StockQuantity = stockQuantity,
            ImagePath = imagePath, // Store original relative path
            Description = descriptionContent
        };
    }

    private async Task<string> ReadProductDescriptionAsync(int productId, string descriptionRelativePath, string baseDirectory)
    {
        string descriptionContent = string.Empty;
        string correctedRelativePath = EnsureDescriptionSuffix(descriptionRelativePath);

        // Construct full path, removing leading '/' or '\' if present for Path.Combine robustness
        string cleanRelativePath = correctedRelativePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string fullDescriptionPath = Path.Combine(baseDirectory, cleanRelativePath);

        _logger.LogDebug("Attempting to read description for Product ID {ProductId}. Original Path: '{OriginalPath}', Corrected Relative Path: '{CorrectedPath}', Full Path: '{FullPath}'",
            productId, descriptionRelativePath, correctedRelativePath, fullDescriptionPath);

        try
        {
            if (File.Exists(fullDescriptionPath))
            {
                descriptionContent = await File.ReadAllTextAsync(fullDescriptionPath);
                _logger.LogInformation("Successfully read description for Product ID {ProductId} from {FullPath}.", productId, fullDescriptionPath);

                // Check and truncate if necessary
                const int maxDescriptionLength = 4000;
                if (descriptionContent.Length > maxDescriptionLength)
                {
                    _logger.LogWarning("Description for Product ID {ProductId} exceeds {MaxLength} characters (length: {Length}). Truncating.",
                        productId, maxDescriptionLength, descriptionContent.Length);
                    descriptionContent = descriptionContent.Substring(0, maxDescriptionLength);
                }
            }
            else
            {
                _logger.LogWarning("Description file not found for Product ID {ProductId} at expected path: {FullPath}. Trying alternate path.", productId, fullDescriptionPath);

                // Try alternate path resolution (relative to current execution directory)
                string altPath = Path.Combine(Directory.GetCurrentDirectory(), cleanRelativePath);
                _logger.LogDebug("Trying alternate description path for Product ID {ProductId}: '{AlternatePath}'", productId, altPath);

                if (File.Exists(altPath))
                {
                    descriptionContent = await File.ReadAllTextAsync(altPath);
                    _logger.LogInformation("Found description file at alternate path for Product ID {ProductId}: {AlternatePath}", productId, altPath);
                    // Apply truncation check here too if needed
                    const int maxDescriptionLength = 4000;
                    if (descriptionContent.Length > maxDescriptionLength)
                    {
                        _logger.LogWarning("Description (alternate path) for Product ID {ProductId} exceeds {MaxLength} characters (length: {Length}). Truncating.",
                           productId, maxDescriptionLength, descriptionContent.Length);
                        descriptionContent = descriptionContent.Substring(0, maxDescriptionLength);
                    }
                }
                else
                {
                    _logger.LogWarning("Description file not found for Product ID {ProductId} at alternate path either: {AlternatePath}. Using empty description.", productId, altPath);
                }
            }
        }
        catch (IOException ioEx)
        {
            _logger.LogError(ioEx, "IO Error reading description file for Product ID {ProductId} from path: {FullPath}. Using empty description.", productId, fullDescriptionPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error reading description file for Product ID {ProductId} from path: {FullPath}. Using empty description.", productId, fullDescriptionPath);
        }

        return descriptionContent;
    }

    private string EnsureDescriptionSuffix(string path)
    {
        const string suffix = "_Description";
        if (string.IsNullOrEmpty(path) || path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            return path; // Return original if null, empty, or already has suffix
        }

        try
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            string directory = Path.GetDirectoryName(path);
            string newFileName = $"{fileNameWithoutExtension}{suffix}{extension}";

            return string.IsNullOrEmpty(directory)
                ? newFileName
                : Path.Combine(directory, newFileName);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Could not process description path '{Path}'. Returning original.", path);
            return path; // Return original path if Path operations fail
        }
    }


    private async Task AddProductsToDatabaseAsync(List<Product> products)
    {
        if (!products.Any())
        {
            _logger.LogWarning("No valid products found in the CSV file to import.");
            return;
        }

        _logger.LogInformation("Attempting to import {ProductCount} valid products...", products.Count);
        int totalImportedCount = 0;

        for (int i = 0; i < products.Count; i += BatchSize)
        {
            var batch = products.Skip(i).Take(BatchSize).ToList(); // Ensure it's a list for logging if needed
            int currentBatchNumber = (i / BatchSize) + 1;
            _logger.LogInformation("Processing Batch {BatchNumber} ({BatchCount} products)...", currentBatchNumber, batch.Count);

            await _context.Products.AddRangeAsync(batch);
            try
            {
                int savedCount = await _context.SaveChangesAsync();
                totalImportedCount += savedCount;
                _logger.LogInformation("Saved Batch {BatchNumber}. {SavedCount} products committed to database.", currentBatchNumber, savedCount);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error saving Batch {BatchNumber}. See inner exception and following entries for details.", currentBatchNumber);
                // Log details about the entities that failed
                foreach (var entry in dbEx.Entries)
                {
                    if (entry.Entity is Product product)
                    {
                        _logger.LogError("Failed DbUpdate for Product: ID={ProductId}, Name='{ProductName}', CategoryId={CategoryId}, Price={Price}, Stock={Stock}, ImagePath='{ImagePath}', Description Length={DescriptionLength}",
                            product.ProductId, product.ProductName, product.CategoryId, product.Price, product.StockQuantity, product.ImagePath, product.Description?.Length ?? 0);
                    }
                    else
                    {
                        _logger.LogError("Failed DbUpdate for entity of type {EntityType}", entry.Entity.GetType().Name);
                    }
                }
                // Consider if you want to stop the entire process or continue with the next batch
                // For now, it continues. Add 'return;' or 'throw;' if failure should halt seeding.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generic error saving Batch {BatchNumber}.", currentBatchNumber);
                // Consider halting as above.
            }
        }
        _logger.LogInformation("Finished importing products. Total successfully committed to database: {ImportedCount} out of {TotalProcessed} processed.", totalImportedCount, products.Count);
    }
}