using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GroceryDeliveryAPI.DTOs
{
    public class OrderCreateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string DeliveryAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        public List<OrderItemCreateDTO> OrderItems { get; set; }
    }
} 