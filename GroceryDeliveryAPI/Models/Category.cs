using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryDeliveryAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        
        public string Name { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; }
    }
}