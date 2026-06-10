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
        /// Lấy voucher theo mã (Public)
        /// </summary>
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(VoucherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVoucherByCode(string code)
        {
            var voucher = await _voucherService.GetVoucherByCodeAsync(code);

            if (voucher == null)
                return NotFound(new { message = "Voucher not found" });

            return Ok(voucher);
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của voucher (Public)
        /// </summary>
        [HttpGet("{code}/validate")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateVoucher(string code)
        {
            var isValid = await _voucherService.IsVoucherValidAsync(code);

            return Ok(new { isValid = isValid });
        }

        /// <summary>
        /// Lấy danh sách tất cả voucher (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(List<VoucherDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllVouchers()
        {
            var vouchers = await _voucherService.GetAllVouchersAsync();
            return Ok(vouchers);
        }

        /// <summary>
        /// Lấy chi tiết voucher theo ID (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(VoucherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetVoucherById(int id)
        {
            var voucher = await _voucherService.GetVoucherByIdAsync(id);

            if (voucher == null)
                return NotFound(new { message = "Voucher not found" });

            return Ok(voucher);
        }

        /// <summary>
        /// Tạo voucher mới (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(VoucherDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var voucher = await _voucherService.CreateVoucherAsync(dto);

                return CreatedAtAction(nameof(GetVoucherById), new { id = voucher.VoucherId }, voucher);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật voucher (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(VoucherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateVoucher(int id, [FromBody] UpdateVoucherDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var voucher = await _voucherService.UpdateVoucherAsync(id, dto);

                return Ok(voucher);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa voucher (Admin only)
        /// </summary>
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
