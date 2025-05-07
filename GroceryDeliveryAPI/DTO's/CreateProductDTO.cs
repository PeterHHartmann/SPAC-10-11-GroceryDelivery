using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.DTO_s
{
    public class CreateProductDTO
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        [StringLength(4000)]
        public string Description { get; set; } = string.Empty;
    }
}
