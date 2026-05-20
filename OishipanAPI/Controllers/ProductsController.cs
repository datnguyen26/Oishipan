using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanAPI.DTOs;
using OishipanAPI.Services;

namespace OishipanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsController(IProductService productService, ICloudinaryService cloudinaryService)
        {
            _productService = productService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetProductsByBrand(int brandId)
        {
            var products = await _productService.GetProductsByBrandAsync(brandId);
            return Ok(products);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _productService.CreateProductAsync(dto);

            if (product == null)
                return BadRequest(new { message = "Failed to create product" });

            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.ProductId = id;
            var product = await _productService.UpdateProductAsync(dto);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);

            if (!success)
                return NotFound(new { message = "Product not found" });

            return Ok(new { message = "Product deleted successfully" });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadProductImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            var imageUrl = await _cloudinaryService.UploadImageAsync(file, "oishipan/products");

            if (imageUrl == null)
                return BadRequest(new { message = "Failed to upload image" });

            var success = await _productService.UpdateProductImageAsync(id, imageUrl);

            if (!success)
                return NotFound(new { message = "Product not found" });

            return Ok(new { message = "Image uploaded successfully", url = imageUrl });
        }
    }
}
