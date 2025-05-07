using GroceryDeliveryAPI.Managers;
using GroceryDeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GroceryDeliveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : Controller
    {
        private readonly DeliveryManager deliveryManager;

        public DeliveriesController(DeliveryManager deliveryManager, OrderManager orderManager)
        {
            this.deliveryManager = deliveryManager;
         
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDeliveries()
        {
            try
            {
                var deliveries = await deliveryManager.GetAllDeliveries();
                return Ok(deliveries);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving deliveries: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryById(int id)
        {
            try
            {
                var delivery = await deliveryManager.GetDeliveryById(id);
                if (delivery == null)
                {
                    return NotFound($"Delivery with ID {id} not found.");
                }
                return Ok(delivery);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving delivery: {ex.Message}");
            }
        }

        // Get delivery by delivery person id
        [HttpGet("deliveryPerson/{deliveryPersonId}")]
        public async Task<IActionResult> GetDeliveriesByDeliveryPersonId(int deliveryPersonId)
        {
            try
            {
                var deliveries = await deliveryManager.GetDeliveriesByDeliveryPersonId(deliveryPersonId);
                if (deliveries == null || deliveries.Count == 0)
                {
                    return NotFound($"No deliveries found for delivery person with ID {deliveryPersonId}.");
                }
                return Ok(deliveries);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving deliveries: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDelivery(int id, [FromBody] Delivery delivery)
        {
            try
            {
                if (delivery == null)
                {
                    return BadRequest("Delivery data is required.");
                }
                var updatedDelivery = await deliveryManager.UpdateDelivery(id, delivery);
                if (updatedDelivery == null)
                {
                    return NotFound($"Delivery with ID {id} not found.");
                }
                return Ok(updatedDelivery);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating delivery: {ex.Message}");
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateDeliveryStatus(int id, Delivery.DeliveryStatus status)
        {
            try
            {
                var updatedDelivery = await deliveryManager.UpdateDeliveryStatus(id, status);
                if (updatedDelivery == null)
                {
                    return NotFound($"Delivery with ID {id} not found.");
                }
                return Ok(updatedDelivery);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating delivery status: {ex.Message}");
            }
        }
    }
}
