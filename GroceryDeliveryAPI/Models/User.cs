using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.Models
{
    public class User
    {
        public enum UserRole
        {
            Customer,
            Admin,
            DeliveryPerson
        }

        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(20)]
        [Required]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        [Required]
        public string Address { get; set; }

        [StringLength(255)]
        [Required]
        public string City { get; set; }

        [StringLength(10)]
        [Required]
        public int ZipCode { get; set; }

        [StringLength(50)]
        [Required]
        public string Country { get; set; }

        [Required]
        public UserRole Role { get; set; }

        // Add discriminator column for TPH inheritance
        protected string Discriminator { get; set; }

        // Add Status with nullable enum since it's only for delivery persons
        public DeliveryPerson.DeliveryPersonStatus? Status { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        // Navigation property
        public virtual ICollection<Order> Orders { get; set; }
    }
}
