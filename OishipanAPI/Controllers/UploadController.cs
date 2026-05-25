using Microsoft.AspNetCore.Mvc;
using OishipanAPI.Services;

namespace OishipanAPI.Controllers
{
    /// <summary>
    /// API cho tải lên ảnh lên Cloudinary
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UploadController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public UploadController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Tải lên ảnh lên Cloudinary
        /// </summary>
        /// <param name="file">File ảnh cần tải lên</param>
        /// <returns>URL của ảnh vừa tải lên</returns>
        /// <response code="200">Tải lên thành công</response>
        /// <response code="400">File không hợp lệ</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "No file provided" });

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { success = false, message = "Invalid file type. Only image files are allowed." });

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { success = false, message = "File size exceeds 5MB limit." });

            var imageUrl = await _cloudinaryService.UploadImageAsync(file, "oishipan/products");

            if (imageUrl == null)
                return BadRequest(new { success = false, message = "Failed to upload image to Cloudinary" });

            return Ok(new { success = true, message = "Image uploaded successfully", url = imageUrl });
        }
    }
}
