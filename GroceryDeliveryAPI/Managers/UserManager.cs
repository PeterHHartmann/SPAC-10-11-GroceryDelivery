using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryDeliveryAPI.Managers
{
    public class UserManager
    {
        private readonly GroceryDeliveryContext _context;

        public UserManager(GroceryDeliveryContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .ToListAsync();

                // Remove sensitive data
                foreach (var user in users)
                {
                    user.Password = null; // Don't return password hashes
                }

                return users;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving users: {ex.Message}", ex);
            }
        }

     
        // Get a user by ID
        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than zero", nameof(id));
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }

                // Remove sensitive data
                user.Password = null; // Don't return password hash

                return user;
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error retrieving user with ID {id}: {ex.Message}", ex);
            }
        }

        // Get a user by email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new ArgumentException("Email cannot be empty", nameof(email));
                }
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with email {email} not found");
                }
                // Remove sensitive data
                user.Password = null; // Don't return password hash
                return user;
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error retrieving user with email {email}: {ex.Message}", ex);
            }
        }

        // Add a new user
        public async Task<User> AddUserAsync(User user, User.UserRole role)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }

                if (string.IsNullOrWhiteSpace(user.FirstName))
                {
                    throw new ArgumentException("First name cannot be empty", nameof(user.FirstName));
                }

                if (string.IsNullOrWhiteSpace(user.LastName))
                {
                    throw new ArgumentException("Last name cannot be empty", nameof(user.LastName));
                }

                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    throw new ArgumentException("Password cannot be empty", nameof(user.Password));
                }
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    throw new ArgumentException("Email cannot be empty", nameof(user.Email));
                }
                // Hash password and set user metadata
                HashPassword(user);
                user.Role = role;
           
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while adding user: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentNullException &&
                                       ex is not ArgumentException &&
                                       ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error adding user: {ex.Message}", ex);
            }
        }

        // Update a user
        public async Task UpdateUserAsync(int id, User updatedUser)
        {
            try
            {
                if (updatedUser == null)
                {
                    throw new ArgumentNullException(nameof(updatedUser), "User cannot be null");
                }

                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than zero", nameof(updatedUser.UserId));
                }

                // Verify user exists
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }

                // Update the existing entity's properties
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.Email = updatedUser.Email;
                if (!string.IsNullOrEmpty(updatedUser.Password))
                {
                    existingUser.Password = updatedUser.Password;
                    HashPassword(existingUser);
                }
                existingUser.PhoneNumber = updatedUser.PhoneNumber;
                existingUser.Address = updatedUser.Address;

                // Save changes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while updating user: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentNullException &&
                                       ex is not ArgumentException &&
                                       ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error updating user with ID {id}: {ex.Message}", ex);
            }
        }

        // Delete a user
        public async Task DeleteUserAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than zero", nameof(id));
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }
                /*
                // Check if user has related inventory transactions
                if (user.InventoryTransactions != null && user.InventoryTransactions.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"Cannot delete user with ID {id} because they have {user.InventoryTransactions.Count} related inventory transactions");
                }
                */
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while deleting user: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error deleting user with ID {id}: {ex.Message}", ex);
            }
        }

        // Hash password
        public void HashPassword(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentException("Password cannot be empty when hashing", nameof(user.Password));
            }

            try
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error hashing password", ex);
            }
        }

        // Verify password
        public bool VerifyPassword(User user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentException("User has no stored password hash", nameof(user.Password));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty when verifying", nameof(password));
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error verifying password", ex);
            }
        }
    }
}
