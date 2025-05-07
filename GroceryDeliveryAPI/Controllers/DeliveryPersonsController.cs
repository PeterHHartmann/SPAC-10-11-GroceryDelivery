using GroceryDeliveryAPI.DTO_s;
using GroceryDeliveryAPI.Managers;
using GroceryDeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GroceryDeliveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryPersonsController : Controller
    {
        private readonly DeliveryPersonsManager deliveryPersonsManager;
        private readonly UserManager _userManager;

        public DeliveryPersonsController(DeliveryPersonsManager deliveryPersonsManager, UserManager userManager)
        {
            this.deliveryPersonsManager = deliveryPersonsManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDeliveryPeople()
        {
            try
            {
                var deliveryPersons = await deliveryPersonsManager.GetAllDeliveryPeopleAsync();
                return Ok(deliveryPersons);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving delivery persons: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryPersonById(int id)
        {
            try
            {
                var deliveryPerson = await deliveryPersonsManager.GetDeliveryPersonByIdAsync(id);
                if (deliveryPerson == null)
                {
                    return NotFound($"Delivery person with ID {id} not found.");
                }
                return Ok(deliveryPerson);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving delivery person: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeliveryPersonAsync(UserDTO userDto)
        {
            try
            {
                // Create the DeliveryPerson directly
                var deliveryPerson = await _userManager.AddUserAsync(userDto, Models.User.UserRole.DeliveryPerson);

                // Cast is safe because we know it's a DeliveryPerson
                return Ok(deliveryPerson);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating delivery person: {ex.Message}");
            }
        }

        /*
        [HttpPost]
        public async  Task<IActionResult> CreateDeliveryPerson([FromBody] DeliveryPerson deliveryPerson)
        {
            try
            {
                if (deliveryPerson == null)
                {
                    return BadRequest("Delivery person data is required.");
                }
                var createdDeliveryPerson = await deliveryPersonsManager.CreateDeliveryPersonAsync(deliveryPerson);
                return CreatedAtAction(nameof(GetDeliveryPersonById), new { id = createdDeliveryPerson.UserId }, createdDeliveryPerson);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating delivery person: {ex.Message}");
            }
        }
        */
        /*
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeliveryPerson(int id, [FromBody] DeliveryPerson deliveryPerson)
        {
            try
            {
                if (id != deliveryPerson.UserId)
                {
                    return BadRequest("ID mismatch.");
                }
                var updatedDeliveryPerson = await deliveryPersonsManager.UpdateDeliveryPersonAsync(id, deliveryPerson);
                if (updatedDeliveryPerson == null)
                {
                    return NotFound($"Delivery person with ID {id} not found.");
                }
                return Ok(updatedDeliveryPerson);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating delivery person: {ex.Message}");
            }
        }
        */
        [HttpDelete("{id}")]
        public IActionResult DeleteDeliveryPerson(int id)
        {
            try
            {
                var deliveryPerson = deliveryPersonsManager.GetDeliveryPersonByIdAsync(id);
                if (deliveryPerson == null)
                {
                    return NotFound($"Delivery person with ID {id} not found.");
                }
                deliveryPersonsManager.DeleteDeliveryPerson(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting delivery person: {ex.Message}");
            }

        }
    }
}
