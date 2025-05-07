using GroceryDeliveryAPI.Models;

namespace GroceryDeliveryAPI.DTO_s
{
    public class UpdateDeliveryDTO
    {
        public int DeliveryId { get; set; }
        public int DeliveryPersonId { get; set; }
        public int OrderId { get; set; }
        public Delivery.DeliveryStatus Status { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? PickupTime { get; set; }
    }
}
