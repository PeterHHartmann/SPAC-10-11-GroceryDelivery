using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.Models
{
    public class Delivery
    {
        [Key]
        public int DeliveryId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("DeliveryPerson")]
        public int DeliveryPersonId { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        public DateTime? PickupTime { get; set; }

        public DateTime? DeliveredTime { get; set; }

        public DateTime? EstimatedDeliveryTime { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual DeliveryPerson DeliveryPerson { get; set; }
    }
}