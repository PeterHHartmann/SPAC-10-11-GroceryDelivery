using System.ComponentModel.DataAnnotations;
using GroceryDeliveryAPI.Models;

namespace GroceryDeliveryAPI.DTO_s
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; set; }
        public string? Password { get; set; }
        public User.UserRole? Role { get; set; }
        public DeliveryPerson.DeliveryPersonStatus? Status { get; set; }
    }
}
