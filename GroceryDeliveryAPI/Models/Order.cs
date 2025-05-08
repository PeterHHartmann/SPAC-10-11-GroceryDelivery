using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GroceryDeliveryAPI.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }

        [StringLength(255)]
        [Required]
        public string DeliveryAddress { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Required]
        public decimal TotalAmount { get; set; }

        [StringLength(20)]
        [Required]
        public string Status { get; set; }

        [StringLength(50)]
        [Required]
        public string PaymentMethod { get; set; }

        public DateTime? DeliveryTime { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<Delivery>? Deliveries { get; set; }
    }
}
