using System;
using System.Collections.Generic;

namespace GroceryDeliveryAPI.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }
} 