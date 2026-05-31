using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanAPI.DTOs;
using OishipanAPI.Services;

namespace OishipanAPI.Controllers
{
    /// <summary>
    /// API cho quản lý sản phẩm bánh
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsController(IProductService productService, ICloudinaryService cloudinaryService)
        {
            _productService = productService;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        /// <returns>Danh sách sản phẩm</returns>
        /// <response code="200">Lấy danh sách thành công</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Lấy chi tiết sản phẩm theo ID
        /// </summary>
        /// <param name="id">ID sản phẩm</param>
        /// <returns>Thông tin chi tiết sản phẩm</returns>
        /// <response code="200">Lấy thông tin thành công</response>
        /// <response code="404">Sản phẩm không tồn tại</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        /// <summary>
        /// Lấy sản phẩm theo danh mục
        /// </summary>
        /// <param name="categoryId">ID danh mục</param>
        /// <returns>Danh sách sản phẩm trong danh mục</returns>
        /// <response code="200">Lấy danh sách thành công</response>
        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        /// <summary>
        /// Lấy sản phẩm theo thương hiệu
        /// </summary>
        /// <param name="brandId">ID thương hiệu</param>
        /// <returns>Danh sách sản phẩm của thương hiệu</returns>
        /// <response code="200">Lấy danh sách thành công</response>
        [HttpGet("brand/{brandId}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductsByBrand(int brandId)
        {
            var products = await _productService.GetProductsByBrandAsync(brandId);
            return Ok(products);
        }

        /// <summary>
        /// Tạo sản phẩm mới (Chỉ Admin/Staff)
        /// </summary>
        /// <param name="dto">Thông tin sản phẩm mới</param>
        /// <returns>Sản phẩm vừa tạo</returns>
        /// <response code="201">Tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="401">Không được phép</response>
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors });
        }

        if (dto == null)
            return BadRequest(new { success = false, message = "Dữ liệu sản phẩm không được để trống" });

        try
        {
            var product = await _productService.CreateProductAsync(dto);

            if (product == null)
                return BadRequest(new { success = false, message = "Không thể tạo sản phẩm. Vui lòng kiểm tra lại danh mục và thương hiệu." });

            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, new { success = true, message = "Tạo sản phẩm thành công", product });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi khi tạo sản phẩm: " + ex.InnerException?.Message ?? ex.Message });
        }
        }

        /// <summary>
        /// Cập nhật sản phẩm (Chỉ Admin/Staff)
        /// </summary>
        /// <param name="id">ID sản phẩm</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>Sản phẩm đã cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="404">Sản phẩm không tồn tại</response>
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            if (id <= 0)
                return BadRequest(new { success = false, message = "ID sản phẩm không hợp lệ" });

            try
            {
                dto.ProductId = id;
                var product = await _productService.UpdateProductAsync(dto);

                if (product == null)
                    return NotFound(new { success = false, message = "Sản phẩm không tồn tại" });

                return Ok(new { success = true, message = "Cập nhật sản phẩm thành công", product });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật sản phẩm: " + ex.InnerException?.Message ?? ex.Message });
            }
        }

        /// <summary>
        /// Xóa sản phẩm (Chỉ Admin/Staff)
        /// </summary>
        /// <param name="id">ID sản phẩm</param>
        /// <returns>Thông báo xóa thành công</returns>
        /// <response code="204">Xóa thành công</response>
        /// <response code="404">Sản phẩm không tồn tại</response>
        /// <response code="401">Không được phép</response>
        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "ID sản phẩm không hợp lệ" });

            try
            {
                var success = await _productService.DeleteProductAsync(id);

                if (!success)
                    return NotFound(new { success = false, message = "Sản phẩm không tồn tại" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa sản phẩm: " + ex.InnerException?.Message ?? ex.Message });
            }
        }

        /// <summary>
        /// Tải lên ảnh sản phẩm (Chỉ Admin/Staff)
        /// </summary>
        /// <param name="id">ID sản phẩm</param>
        /// <param name="file">File ảnh</param>
        /// <returns>URL ảnh vừa tải lên</returns>
        /// <response code="200">Tải lên thành công</response>
        /// <response code="400">File không hợp lệ</response>
        /// <response code="404">Sản phẩm không tồn tại</response>
        /// <response code="401">Không được phép</response>
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("{id}/upload-image")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadProductImage(int id, IFormFile file)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "ID sản phẩm không hợp lệ" });

            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "Vui lòng chọn file ảnh" });

            if (file.Length > 5 * 1024 * 1024) // 5MB
                return BadRequest(new { success = false, message = "Kích thước file không được vượt quá 5MB" });

            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(file, "oishipan/products");

                if (imageUrl == null)
                    return BadRequest(new { success = false, message = "Không thể tải lên ảnh. Vui lòng kiểm tra định dạng file." });

                var success = await _productService.UpdateProductImageAsync(id, imageUrl);

                if (!success)
                    return NotFound(new { success = false, message = "Sản phẩm không tồn tại" });

                return Ok(new { success = true, message = "Tải lên ảnh thành công", url = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tải lên ảnh: " + ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
