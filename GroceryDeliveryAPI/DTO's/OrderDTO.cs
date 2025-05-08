using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }
}