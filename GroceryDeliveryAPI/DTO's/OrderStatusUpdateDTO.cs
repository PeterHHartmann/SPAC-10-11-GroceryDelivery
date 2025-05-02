using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.DTOs
{
    public class OrderStatusUpdateDTO
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; }
    }
} 