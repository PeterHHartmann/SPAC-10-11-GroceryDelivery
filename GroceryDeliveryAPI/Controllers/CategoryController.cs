using GroceryDeliveryAPI.DTO_s;
using GroceryDeliveryAPI.Managers;
using Microsoft.AspNetCore.Mvc;

namespace GroceryDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly CategoryManager _manager;
        public CategoryController(CategoryManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _manager.GetCategoriesAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found.");
            }
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _manager.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(category);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var category = await _manager.GetCategoryByNameAsync(name);
            if (category == null)
            {
                return NotFound($"Category with name {name} not found.");
            }
            return Ok(category);
        }

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetCategoryByIdWithProducts(int id)
        {
            var category = await _manager.GetCategoryByIdWithProductsAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Category data is null.");
            }
            var createdCategory = await _manager.CreateCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Category data is null.");
            }
            var updatedCategory = await _manager.UpdateCategoryAsync(id, categoryDto);
            if (updatedCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _manager.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
