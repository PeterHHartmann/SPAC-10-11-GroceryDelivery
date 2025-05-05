using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.Models
{
    public class DeliveryPerson : User
    {
        public enum DeliveryPersonStatus
        {
            Available,       // Ready to accept new deliveries
            Busy,           // Currently on a delivery
            OnBreak,        // Taking a break, not available for deliveries
            Offline,        // Not working/signed out
            Inactive        // Account is disabled or suspended
        }

        public new DeliveryPersonStatus Status
        {
            get => base.Status ?? DeliveryPersonStatus.Offline;
            set => base.Status = value;
        }

        // Navigation property
        public virtual ICollection<Delivery>? Deliveries { get; set; }

        public DeliveryPerson()
        {
            Role = UserRole.DeliveryPerson; // Set default role
            Status = DeliveryPersonStatus.Available; // Set default status
        }
    }
}
