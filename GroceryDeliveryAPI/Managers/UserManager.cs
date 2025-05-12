using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.DTO_s;
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

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new UserDTO
                    {
                        Id = u.UserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Address = u.Address,
                        Role = u.Role,
                        Password = null // Never expose password
                    })
                    .ToListAsync();

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

                return user;
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error retrieving user with email {email}: {ex.Message}", ex);
            }
        }

        // Add a new user
        public async Task<User> AddUserAsync(UserDTO user, User.UserRole role)
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
                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    throw new ArgumentException("Phone number cannot be empty", nameof(user.PhoneNumber));
                }
                if (string.IsNullOrWhiteSpace(user.Address))
                {
                    throw new ArgumentException("Address cannot be empty", nameof(user.Address));
                }
                // Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException($"User with email {user.Email} already exists");
                }
                // Convert to user entity
                User newUser;

                if (role == User.UserRole.DeliveryPerson)
                {
                    newUser = new DeliveryPerson
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address,
                        City = user.City,
                        ZipCode = user.ZipCode,
                        Country = user.Country,
                        RegistrationDate = DateTime.UtcNow,
                        Role = role,
                        Password = user.Password,
                        Status = DeliveryPerson.DeliveryPersonStatus.Available,
                    };
                }
                else
                {
                    newUser = new User
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address,
                        City = user.City,
                        ZipCode = user.ZipCode,
                        Country = user.Country,
                        RegistrationDate = DateTime.UtcNow,
                        Role = role,
                        Password = user.Password
                    };
                }

                // Hash password and set user metadata
                HashPassword(newUser);
                user.Role = role;

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return newUser;
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
        public async Task UpdateUserAsync(int id, UpdateUserDTO updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    throw new ArgumentNullException(nameof(updateDto), "Update DTO cannot be null");
                }

                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than zero", nameof(id));
                }

                // Verify user exists
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }

                // Update only non-null properties
                if (!string.IsNullOrWhiteSpace(updateDto.FirstName))
                    existingUser.FirstName = updateDto.FirstName;

                if (!string.IsNullOrWhiteSpace(updateDto.LastName))
                    existingUser.LastName = updateDto.LastName;

                if (!string.IsNullOrWhiteSpace(updateDto.Email))
                    existingUser.Email = updateDto.Email;

                if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber))
                    existingUser.PhoneNumber = updateDto.PhoneNumber;

                if (!string.IsNullOrWhiteSpace(updateDto.Address))
                    existingUser.Address = updateDto.Address;

                // Handle password update
                if (!string.IsNullOrEmpty(updateDto.Password))
                {
                    existingUser.Password = updateDto.Password;
                    HashPassword(existingUser);
                }

                // Handle role and status updates
                if (updateDto.Role.HasValue)
                {
                    // Handle conversion to/from DeliveryPerson
                    if (updateDto.Role == User.UserRole.DeliveryPerson && !(existingUser is DeliveryPerson))
                    {
                        // Convert to delivery person
                        var newDeliveryPerson = new DeliveryPerson
                        {
                            UserId = existingUser.UserId,
                            FirstName = existingUser.FirstName,
                            LastName = existingUser.LastName,
                            Email = existingUser.Email,
                            Password = existingUser.Password,
                            PhoneNumber = existingUser.PhoneNumber,
                            Address = existingUser.Address,
                            City = existingUser.City,
                            ZipCode = existingUser.ZipCode,
                            Country = existingUser.Country,
                            Role = User.UserRole.DeliveryPerson,
                            Status = DeliveryPerson.DeliveryPersonStatus.Available,
                            RegistrationDate = existingUser.RegistrationDate
                        };

                        _context.Users.Remove(existingUser);
                        await _context.Users.AddAsync(newDeliveryPerson);
                        existingUser = newDeliveryPerson;
                    }
                    else
                    {
                        existingUser.Role = updateDto.Role.Value;
                    }
                }

                // Update status for delivery person
                if (existingUser is DeliveryPerson existingDeliveryPerson && updateDto.Status.HasValue)
                {
                    existingDeliveryPerson.Status = updateDto.Status.Value;
                }

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
