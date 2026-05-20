using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanAPI.DTOs;
using OishipanAPI.Services;

namespace OishipanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetVoucherByCode(string code)
        {
            var voucher = await _voucherService.GetVoucherByCodeAsync(code);

            if (voucher == null)
                return NotFound(new { message = "Voucher not found" });

            return Ok(voucher);
        }

        [HttpGet("{code}/validate")]
        public async Task<IActionResult> ValidateVoucher(string code)
        {
            var isValid = await _voucherService.IsVoucherValidAsync(code);

            return Ok(new { isValid = isValid });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllVouchers()
        {
            var vouchers = await _voucherService.GetAllVouchersAsync();
            return Ok(vouchers);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var voucher = await _voucherService.CreateVoucherAsync(dto);

            if (voucher == null)
                return BadRequest(new { message = "Failed to create voucher" });

            return CreatedAtAction(nameof(GetVoucherByCode), new { code = voucher.Code }, voucher);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var success = await _voucherService.DeleteVoucherAsync(id);

            if (!success)
                return NotFound(new { message = "Voucher not found" });

            return Ok(new { message = "Voucher deleted successfully" });
        }
    }
}
