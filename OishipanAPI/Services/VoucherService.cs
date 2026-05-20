using OishipanAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Oishipan.Models;

namespace OishipanAPI.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly OishipanContext _context;

        public VoucherService(OishipanContext context)
        {
            _context = context;
        }

        public async Task<VoucherDto> GetVoucherByCodeAsync(string code)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code == code.ToUpper());

            if (voucher == null)
                return null;

            return new VoucherDto
            {
                VoucherId = voucher.VoucherId,
                Code = voucher.Code,
                DiscountValue = voucher.DiscountValue,
                ExpiryDate = voucher.ExpiryDate
            };
        }

        public async Task<List<VoucherDto>> GetAllVouchersAsync()
        {
            return await _context.Vouchers
                .Select(v => new VoucherDto
                {
                    VoucherId = v.VoucherId,
                    Code = v.Code,
                    DiscountValue = v.DiscountValue,
                    ExpiryDate = v.ExpiryDate
                })
                .ToListAsync();
        }

        public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto)
        {
            var voucher = new Voucher
            {
                Code = dto.Code.ToUpper(),
                DiscountValue = dto.DiscountValue,
                ExpiryDate = dto.ExpiryDate
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return new VoucherDto
            {
                VoucherId = voucher.VoucherId,
                Code = voucher.Code,
                DiscountValue = voucher.DiscountValue,
                ExpiryDate = voucher.ExpiryDate
            };
        }

        public async Task<bool> DeleteVoucherAsync(int voucherId)
        {
            var voucher = await _context.Vouchers.FindAsync(voucherId);

            if (voucher == null)
                return false;

            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsVoucherValidAsync(string code)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code == code.ToUpper());

            if (voucher == null)
                return false;

            // Check if voucher has not expired
            return voucher.ExpiryDate >= DateTime.Now;
        }
    }
}
