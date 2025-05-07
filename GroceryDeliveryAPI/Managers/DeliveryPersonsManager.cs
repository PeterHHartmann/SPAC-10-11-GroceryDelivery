using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryDeliveryAPI.Managers
{
    public class DeliveryPersonsManager
    {
        private readonly GroceryDeliveryContext _context;

        public DeliveryPersonsManager(GroceryDeliveryContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllDeliveryPeopleAsync()
        {
            try
            {
                var deliveryPeople = await _context.Users.Where(u => u.Role == User.UserRole.DeliveryPerson).ToListAsync();

                return deliveryPeople;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving users: {ex.Message}", ex);
            }

        }

        public async Task<User> GetDeliveryPersonByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("ID must be greater than zero", nameof(id));
                }

                var deliveryPerson = await _context.Users.Where(u => u.Role == User.UserRole.DeliveryPerson)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (deliveryPerson == null)
                {
                    throw new InvalidOperationException($"DeliveryPerson with ID {id} not found");
                }

                return deliveryPerson;
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error retrieving deliveryPerson with ID {id}: {ex.Message}", ex);
            }
        }
        /*
        public async Task<DeliveryPerson> CreateDeliveryPersonAsync(DeliveryPerson deliveryPerson)
        {
            try
            {
                if (deliveryPerson == null)
                {
                    throw new ArgumentNullException(nameof(deliveryPerson), "DeliveryPerson cannot be null");
                }
                _context.DeliveryPersons.Add(deliveryPerson);
                await _context.SaveChangesAsync();
                return deliveryPerson;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Error creating deliveryPerson: {ex.Message}", ex);
            }
        }
        */
        /*
        public async Task<bool> UpdateDeliveryPersonAsync(int id, DeliveryPerson deliveryPerson)
        {
            try
            {
                if (id != deliveryPerson.UserId)
                {
                    throw new ArgumentException("ID mismatch", nameof(id));
                }
                var existingDeliveryPerson = await _context.Users.FindAsync(id);
                if (existingDeliveryPerson == null)
                {
                    throw new InvalidOperationException($"DeliveryPerson with ID {id} not found");
                }
                existingDeliveryPerson.FirstName = deliveryPerson.FirstName;
                existingDeliveryPerson.LastName = deliveryPerson.LastName;
                existingDeliveryPerson.PhoneNumber = deliveryPerson.PhoneNumber;
                existingDeliveryPerson.Email = deliveryPerson.Email;
                existingDeliveryPerson.Status = deliveryPerson.Status; // Add this line
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException($"Error updating deliveryPerson: {ex.Message}", ex);
            }
        }
        */
        public Task DeleteDeliveryPerson(int id)
        {
            var deliveryPerson = _context.Users.Find(id);
            if (deliveryPerson == null)
            {
                throw new InvalidOperationException($"DeliveryPerson with ID {id} not found");
            }
            _context.Users.Remove(deliveryPerson);
            return _context.SaveChangesAsync();
        }
    }
}
