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

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Remove the Status property declaration since it's in the base class
        // but add a non-nullable accessor
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
            Name = $"{FirstName} {LastName}"; // Compose name from User properties
        }
    }
}
