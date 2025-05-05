using GroceryDeliveryAPI.DTOs;
using GroceryDeliveryAPI.Models;

namespace GroceryDeliveryAPI.DTO_s
{
    public class DeliveryDTO
    {
        public int DeliveryId { get; set; }
        public int DeliveryPersonId { get; set; }
        public int OrderId { get; set; }
        public Delivery.DeliveryStatus Status { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveredTime { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public string DeliveryPersonName { get; set; }
        public OrderDTO Order { get; set; }
    }
}
