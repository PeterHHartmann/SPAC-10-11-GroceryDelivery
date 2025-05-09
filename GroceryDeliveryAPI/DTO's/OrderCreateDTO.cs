using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GroceryDeliveryAPI.DTOs
{
    public class OrderCreateDTO
    {
        [Required]
        public int UserId { get; set; }

        [StringLength(255)]
        [Required]
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

        [Required]
        public List<OrderItemCreateDTO> OrderItems { get; set; }
    }
}