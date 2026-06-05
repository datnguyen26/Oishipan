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

            return MapToDto(voucher);
        }

        public async Task<VoucherDto> GetVoucherByIdAsync(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);

            if (voucher == null)
                return null;

            return MapToDto(voucher);
        }

        public async Task<List<VoucherDto>> GetAllVouchersAsync()
        {
            return await _context.Vouchers
                .OrderByDescending(v => v.CreatedAt)
                .Select(v => MapToDto(v))
                .ToListAsync();
        }

        public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto)
        {
            // Kiểm tra trùng mã
            var existingVoucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code == dto.Code.ToUpper());

            if (existingVoucher != null)
                throw new InvalidOperationException($"Mã voucher {dto.Code} đã tồn tại!");

            var voucher = new Voucher
            {
                Code = dto.Code.ToUpper(),
                Name = dto.Name,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MaxDiscount = dto.MaxDiscount,
                MinOrderValue = dto.MinOrderValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UsageLimit = dto.UsageLimit,
                UsageCount = 0,
                Status = dto.Status,
                Category = dto.Category,
                CreatedAt = DateTime.Now
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return MapToDto(voucher);
        }

        public async Task<VoucherDto> UpdateVoucherAsync(int id, UpdateVoucherDto dto)
        {
            var voucher = await _context.Vouchers.FindAsync(id);

            if (voucher == null)
                throw new InvalidOperationException($"Voucher với ID {id} không tồn tại!");

            // Cập nhật các trường nếu có giá trị
            if (!string.IsNullOrWhiteSpace(dto.Name))
                voucher.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                voucher.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.DiscountType))
                voucher.DiscountType = dto.DiscountType;

            if (dto.DiscountValue.HasValue && dto.DiscountValue > 0)
                voucher.DiscountValue = dto.DiscountValue.Value;

            if (dto.MaxDiscount.HasValue && dto.MaxDiscount >= 0)
                voucher.MaxDiscount = dto.MaxDiscount.Value;

            if (dto.MinOrderValue.HasValue && dto.MinOrderValue >= 0)
                voucher.MinOrderValue = dto.MinOrderValue.Value;

            if (dto.StartDate.HasValue)
                voucher.StartDate = dto.StartDate.Value;

            if (dto.EndDate.HasValue)
                voucher.EndDate = dto.EndDate.Value;

            if (dto.UsageLimit.HasValue && dto.UsageLimit > 0)
                voucher.UsageLimit = dto.UsageLimit.Value;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                voucher.Status = dto.Status;

            if (!string.IsNullOrWhiteSpace(dto.Category))
                voucher.Category = dto.Category;

            voucher.UpdatedAt = DateTime.Now;

            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();

            return MapToDto(voucher);
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

            // Kiểm tra: status = active, ngày còn hiệu lực, và chưa vượt hạn mức sử dụng
            return voucher.Status == "active" 
                && voucher.StartDate <= DateTime.Now 
                && voucher.EndDate >= DateTime.Now 
                && voucher.UsageCount < voucher.UsageLimit;
        }

        private VoucherDto MapToDto(Voucher voucher)
        {
            return new VoucherDto
            {
                VoucherId = voucher.VoucherId,
                Code = voucher.Code,
                Name = voucher.Name,
                Description = voucher.Description,
                DiscountType = voucher.DiscountType,
                DiscountValue = voucher.DiscountValue,
                MaxDiscount = voucher.MaxDiscount,
                MinOrderValue = voucher.MinOrderValue,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                UsageLimit = voucher.UsageLimit,
                UsageCount = voucher.UsageCount,
                Status = voucher.Status,
                Category = voucher.Category,
                CreatedAt = voucher.CreatedAt,
                UpdatedAt = voucher.UpdatedAt
            };
        }
    }
}

