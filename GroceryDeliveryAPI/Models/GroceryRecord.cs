namespace GroceryDeliveryAPI.Models
{
    public class GroceryRecord
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }
        public decimal price { get; set; }
        public int stock_quantity { get; set; }
        public string image_path { get; set; }
        public string description_path { get; set; }
    }
}
