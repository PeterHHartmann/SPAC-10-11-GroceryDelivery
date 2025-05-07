using GroceryDeliveryAPI.DTOs;

namespace GroceryDeliveryAPI.DTO_s
{
    public class CategoryWithProductsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductDTO> Products { get; set; }
    }
}
