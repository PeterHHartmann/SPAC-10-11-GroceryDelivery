using GroceryDeliveryAPI.Models;

namespace GroceryDeliveryAPI.DTO_s
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public User.UserRole Role { get; set; }
        public string Password { get; set; }

    }
}
