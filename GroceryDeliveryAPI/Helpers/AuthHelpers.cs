using GroceryDeliveryAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GroceryDeliveryAPI.Helpers
{
    public class AuthHelpers
    {
        private readonly IConfiguration _configuration;

        public AuthHelpers(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Generate JWT Token
        public string GenerateJWTToken(User user)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured")));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim> {
            new Claim("id", user.UserId.ToString()),
            new Claim("name", user.FirstName + " " + user.LastName),
            new Claim("role", user.Role.ToString())
        };

                var jwtToken = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }
            catch (Exception e)
            {
                throw new Exception("Error generating JWT token", e);
            }
        }
    }
}
