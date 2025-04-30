using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.Models
{
    public class DeliveryPerson
    {
        [Key]
        public int DeliveryPersonId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        // Navigation property
        public virtual ICollection<Delivery>? Deliveries { get; set; }
    }
}
