using GroceryDeliveryAPI.Helpers;
using GroceryDeliveryAPI.Managers;
using GroceryDeliveryAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly UserManager _userManager;
        private readonly AuthHelpers _authHelpers;

        public LoginController(UserManager userManager, AuthHelpers authHelpers)
        {
            _userManager = userManager;
            _authHelpers = authHelpers;
        }

        // POST: api/login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                var user = await _userManager.GetUserByEmailAsync(loginModel.Email); // Get user by email
                if (user == null)
                {
                    return Unauthorized("Invalid email"); // Return unauthorized if user not found
                }
                if (!_userManager.VerifyPassword(user, loginModel.Password)) // Verify password
                {
                    return Unauthorized("Invalid password"); // Return unauthorized if password is incorrect
                }
                var token = _authHelpers.GenerateJWTToken(user); // Generate JWT token
                return Ok(new { token }); // Return token
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while logging in: {ex.Message}");
            }
        }

    }
}
