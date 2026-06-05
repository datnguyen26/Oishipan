using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanAPI.DTOs;
using OishipanAPI.Services;
using System.Security.Claims;

namespace OishipanAPI.Controllers
{
    /// <summary>
    /// API cho quản lý người dùng (Admin/Staff)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Staff")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Tìm kiếm người dùng theo tên hoặc email
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách người dùng khớp</returns>
        /// <response code="200">Thành công</response>
        /// <response code="400">Từ khóa tìm kiếm không hợp lệ</response>
        /// <response code="401">Không được phép</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return BadRequest(new { success = false, message = "Từ khóa tìm kiếm phải có ít nhất 2 ký tự" });
            }

            try
            {
                var users = await _userService.SearchUsersAsync(searchTerm.Trim());
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tìm kiếm người dùng với từ khóa: {SearchTerm}", searchTerm);
                return StatusCode(500, new { success = false, message = "Lỗi khi tìm kiếm người dùng" });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng (có phân trang, lọc, tìm kiếm)
        /// </summary>
        /// <param name="role">Lọc theo role (Admin, Staff, User)</param>
        /// <param name="status">Lọc theo trạng thái (true=active, false=inactive)</param>
        /// <param name="page">Trang hiện tại (mặc định 1)</param>
        /// <param name="pageSize">Số bản ghi mỗi trang (mặc định 20)</param>
        /// <param name="search">Tìm kiếm theo tên/email/số điện thoại</param>
        /// <returns>Danh sách người dùng</returns>
        /// <response code="200">Thành công</response>
        /// <response code="401">Không được phép (chỉ Admin/Staff)</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string role = null,
            [FromQuery] bool? status = null,
            [FromQuery] string search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _userService.GetAllUsersAsync(role, status, search, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tải danh sách người dùng");
                return StatusCode(500, new { success = false, message = "Lỗi khi tải danh sách người dùng" });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết một người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thông tin người dùng</returns>
        /// <response code="200">Thành công</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        /// <response code="401">Không được phép</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tải thông tin người dùng ID: {UserId}", id);
                return StatusCode(500, new { success = false, message = "Lỗi khi tải thông tin người dùng" });
            }
        }

        /// <summary>
        /// Tạo người dùng mới (chỉ Admin)
        /// </summary>
        /// <param name="request">Thông tin người dùng mới</param>
        /// <returns>Thông tin người dùng đã tạo</returns>
        /// <response code="201">Tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ hoặc email đã tồn tại</response>
        /// <response code="401">Không được phép (chỉ Admin)</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            try
            {
                var user = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, new { success = true, message = "Tạo người dùng thành công", user });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo người dùng mới");
                return StatusCode(500, new { success = false, message = "Lỗi khi tạo người dùng" });
            }
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Thông tin người dùng đã cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="401">Không được phép hoặc quyền không đủ</response>
        /// <response code="403">Staff không thể thay đổi role</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                // Check authorization for role changes - only Admin can change roles
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Staff")
                {
                    var currentUser = await _userService.GetUserByIdAsync(id);
                    if (currentUser != null && currentUser.Role != request.Role)
                    {
                        return Forbid("Staff không có quyền thay đổi vai trò người dùng");
                    }
                }

                var user = await _userService.UpdateUserAsync(id, request);
                if (user == null)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return Ok(new { success = true, message = "Cập nhật người dùng thành công", user });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật người dùng ID: {UserId}", id);
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật người dùng" });
            }
        }

        /// <summary>
        /// Thay đổi mật khẩu người dùng (chỉ Admin)
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <param name="request">Mật khẩu mới</param>
        /// <returns>Thông báo kết quả</returns>
        /// <response code="200">Đổi mật khẩu thành công</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="401">Không được phép</response>
        [HttpPost("{id}/change-password")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var result = await _userService.ChangePasswordAsync(id, request.NewPassword);
                if (!result)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return Ok(new { success = true, message = "Thay đổi mật khẩu thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi thay đổi mật khẩu cho người dùng ID: {UserId}", id);
                return StatusCode(500, new { success = false, message = "Lỗi khi thay đổi mật khẩu" });
            }
        }

        /// <summary>
        /// Khóa/Mở khóa người dùng (chỉ Admin)
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Trạng thái mới của người dùng</returns>
        /// <response code="200">Thành công</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        /// <response code="401">Không được phép</response>
        [HttpPost("{id}/toggle-status")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var user = await _userService.ToggleUserStatusAsync(id);
                if (user == null)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return Ok(new { success = true, message = $"Người dùng đã được {(user.Status ? "kích hoạt" : "khóa")}", user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật trạng thái người dùng ID: {UserId}", id);
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật trạng thái" });
            }
        }

        /// <summary>
        /// Xóa người dùng (chỉ Admin)
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thông báo kết quả</returns>
        /// <response code="204">Xóa thành công</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        /// <response code="401">Không được phép</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa người dùng ID: {UserId}", id);
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa người dùng" });
            }
        }

        /// <summary>
        /// Tải lên ảnh đại diện người dùng (chỉ Admin)
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <param name="file">File ảnh</param>
        /// <returns>Thông báo kết quả</returns>
        /// <response code="200">Tải lên thành công</response>
        /// <response code="400">File không hợp lệ</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        [HttpPost("{id}/upload-image")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UploadUserImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File không hợp lệ" });

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Người dùng không tồn tại" });

            return Ok(new { message = "User image upload feature - to be implemented with Cloudinary" });
        }
    }
}
