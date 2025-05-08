using System;
using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.DTOs
{
    public class OrderUpdateDTO
    {
        [Required]
        [StringLength(255)]
        public string DeliveryAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        public DateTime? DeliveryTime { get; set; }
    }
} 