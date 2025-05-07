using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.Models
{
    public class Delivery
    {
        public enum DeliveryStatus
        {
            Pending,        // Delivery is scheduled but not yet picked up
            InProgress,     // Delivery is currently being made
            Completed,      // Delivery has been successfully completed
            Cancelled       // Delivery was cancelled
        }

        [Key]
        public int DeliveryId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("DeliveryPerson")]
        public int DeliveryPersonId { get; set; }

        [StringLength(20)]
        public DeliveryStatus Status { get; set; }

        public DateTime? PickupTime { get; set; }

        public DateTime? DeliveredTime { get; set; }

        public DateTime? EstimatedDeliveryTime { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual DeliveryPerson DeliveryPerson { get; set; }
    }
}