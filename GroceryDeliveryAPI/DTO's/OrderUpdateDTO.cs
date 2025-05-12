using System;
using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.DTOs
{
    public class OrderUpdateDTO
    {
        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(255)]
        [Required]
        public string City { get; set; }

        [Required]
        public int ZipCode { get; set; }

        [StringLength(50)]
        [Required]
        public string Country { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        public DateTime? DeliveryTime { get; set; }
    }
}