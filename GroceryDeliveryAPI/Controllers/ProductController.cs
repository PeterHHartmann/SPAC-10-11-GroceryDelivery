using Microsoft.AspNetCore.Mvc;
using GroceryDeliveryAPI.DTOs;
using GroceryDeliveryAPI.Managers;

namespace GroceryDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductManager _productManager;

        public ProductController(ProductManager productManager)
        {
            _productManager = productManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? categoryId = null)
        {
            var products = await _productManager.GetProductsAsync(page, pageSize, searchTerm, categoryId);
            var totalCount = await _productManager.GetTotalProductCountAsync(searchTerm, categoryId);

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Total-Pages", Math.Ceiling((double)totalCount / pageSize).ToString());

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productManager.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] CreateProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProduct = await _productManager.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedProduct = await _productManager.UpdateProductAsync(id, productDto);
            if (updatedProduct == null)
            {
                return NotFound();
            }

            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productManager.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("upload-image")]
        public async Task<ActionResult<string>> UploadImage([FromBody] ImageUploadModel model)
        {
            if (string.IsNullOrEmpty(model.Base64Image) || string.IsNullOrEmpty(model.FileName))
            {
                return BadRequest("Image data and filename are required");
            }

            try
            {
                var imagePath = await _productManager.SaveProductImageAsync(model.Base64Image, model.FileName);
                return Ok(new { ImagePath = imagePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class ImageUploadModel
    {
        public string Base64Image { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
} 