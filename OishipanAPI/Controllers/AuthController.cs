using Microsoft.AspNetCore.Mvc;
using OishipanAPI.DTOs;
using OishipanAPI.Services;

namespace OishipanAPI.Controllers
{
    /// <summary>
    /// API cho quản lý xác thực người dùng (login, register, profile)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        /// <param name="request">Thông tin đăng nhập (email, password)</param>
        /// <returns>JWT token nếu thành công</returns>
        /// <response code="200">Đăng nhập thành công</response>
        /// <response code="401">Email hoặc mật khẩu không chính xác</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            var response = await _authService.LoginAsync(request);

            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="request">Thông tin đăng ký (email, password, fullName, phoneNumber)</param>
        /// <returns>Thông báo đăng ký thành công</returns>
        /// <response code="200">Đăng ký thành công</response>
        /// <response code="400">Email đã tồn tại hoặc dữ liệu không hợp lệ</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            var response = await _authService.RegisterAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Lấy thông tin hồ sơ người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Thông tin hồ sơ người dùng</returns>
        /// <response code="200">Lấy thông tin thành công</response>
        /// <response code="404">Người dùng không tồn tại</response>
        [HttpGet("profile/{userId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="request">Thông tin cần cập nhật</param>
        /// <returns>Thông báo cập nhật thành công</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="404">Người dùng không tồn tại</response>
        [HttpPut("update-profile/{userId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UserDto request)
        {
            var success = await _authService.UpdateUserAsync(userId, request.FullName, request.PhoneNumber, request.Address);

            if (!success)
                return NotFound();

            return Ok(new { message = "Profile updated successfully" });
        }
    }
}
