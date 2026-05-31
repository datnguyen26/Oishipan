using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanAPI.DTOs;
using OishipanAPI.Services;

namespace OishipanAPI.Controllers
{
    /// <summary>
    /// API cho quản lý người dùng (Admin only)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Staff")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng (có phân trang, lọc, tìm kiếm)
        /// </summary>
        /// <param name="role">Lọc theo role (Admin, Staff, User)</param>
        /// <param name="status">Lọc theo trạng thái (true=active, false=inactive)</param>
        /// <param name="page">Trang hiện tại (mặc định 1)</param>
        /// <param name="pageSize">Số bản ghi mỗi trang (mặc định 20)</param>
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
                var result = await _userService.GetAllUsersAsync(role, status, search, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tải danh sách người dùng", error = ex.Message });
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
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tải thông tin người dùng", error = ex.Message });
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
        /// <response code="401">Không được phép</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            try
            {
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
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo người dùng mới (chỉ Admin)
        /// </summary>
        /// <param name="request">Thông tin người dùng mới</param>
        /// <returns>Thông tin người dùng đã tạo</returns>
        /// <response code="201">Tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ hoặc email đã tồn tại</response>
        /// <response code="401">Không được phép</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
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
                return StatusCode(500, new { success = false, message = "Lỗi khi tạo người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa người dùng (chỉ Admin)
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thông báo kết quả</returns>
        /// <response code="200">Xóa thành công</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        /// <response code="401">Không được phép</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return Ok(new { success = true, message = "Xóa người dùng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// Khóa/Mở khóa người dùng
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
                var result = await _userService.ToggleUserStatusAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                var user = await _userService.GetUserByIdAsync(id);
                return Ok(new { success = true, message = $"Người dùng đã được {(user.Status ? "kích hoạt" : "khóa")}", user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật trạng thái", error = ex.Message });
            }
        }

        /// <summary>
        /// Tìm kiếm người dùng theo tên hoặc email
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách người dùng khớp</returns>
        /// <response code="200">Thành công</response>
        /// <response code="401">Không được phép</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            try
            {
                var users = await _userService.SearchUsersAsync(searchTerm);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tìm kiếm người dùng", error = ex.Message });
            }
        }
    }
}
