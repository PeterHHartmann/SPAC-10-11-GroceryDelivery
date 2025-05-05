using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

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

        [Required]
        public DateTime RegistrationDate { get; set; }

        // Navigation property
        public virtual ICollection<Order> Orders { get; set; }
    }
}
