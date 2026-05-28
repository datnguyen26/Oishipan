using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanAPI.DTOs;
using OishipanAPI.Services;

namespace OishipanAPI.Controllers
{
    /// <summary>
    /// API cho quản lý mã khuyến mại (voucher)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        /// <summary>
        /// Lấy voucher theo mã
        /// </summary>
        /// <param name="code">Mã voucher</param>
        /// <returns>Thông tin voucher</returns>
        /// <response code="200">Lấy thông tin thành công</response>
        /// <response code="404">Voucher không tồn tại</response>
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVoucherByCode(string code)
        {
            var voucher = await _voucherService.GetVoucherByCodeAsync(code);

            if (voucher == null)
                return NotFound(new { message = "Voucher not found" });

            return Ok(voucher);
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của voucher
        /// </summary>
        /// <param name="code">Mã voucher</param>
        /// <returns>Trạng thái hợp lệ</returns>
        /// <response code="200">Kiểm tra thành công</response>
        [HttpGet("{code}/validate")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateVoucher(string code)
        {
            var isValid = await _voucherService.IsVoucherValidAsync(code);

            return Ok(new { isValid = isValid });
        }

        /// <summary>
        /// Lấy danh sách tất cả voucher (Chỉ Admin)
        /// </summary>
        /// <returns>Danh sách voucher</returns>
        /// <response code="200">Lấy danh sách thành công</response>
        /// <response code="401">Không được phép</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllVouchers()
        {
            var vouchers = await _voucherService.GetAllVouchersAsync();
            return Ok(vouchers);
        }

        /// <summary>
        /// Tạo voucher mới (Chỉ Admin)
        /// </summary>
        /// <param name="dto">Thông tin voucher</param>
        /// <returns>Voucher vừa tạo</returns>
        /// <response code="201">Tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="401">Không được phép</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var voucher = await _voucherService.CreateVoucherAsync(dto);

            if (voucher == null)
                return BadRequest(new { message = "Failed to create voucher" });

            return CreatedAtAction(nameof(GetVoucherByCode), new { code = voucher.Code }, voucher);
        }

        /// <summary>
        /// Xóa voucher (Chỉ Admin)
        /// </summary>
        /// <param name="id">ID voucher</param>
        /// <returns>Thông báo xóa thành công</returns>
        /// <response code="200">Xóa thành công</response>
        /// <response code="404">Voucher không tồn tại</response>
        /// <response code="401">Không được phép</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var success = await _voucherService.DeleteVoucherAsync(id);

            if (!success)
                return NotFound(new { message = "Voucher not found" });

            return Ok(new { message = "Voucher deleted successfully" });
        }
    }
}
