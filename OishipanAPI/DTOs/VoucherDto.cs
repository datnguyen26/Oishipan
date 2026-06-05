namespace OishipanAPI.DTOs
{
    /// <summary>
    /// DTO cho Voucher - Dữ liệu trả về khi GET
    /// </summary>
    public class VoucherDto
    {
        public int VoucherId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string DiscountType { get; set; } = "percentage"; // percentage hoặc fixed
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscount { get; set; }
        public decimal MinOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public string Status { get; set; } = "active"; // active, scheduled, expired
        public string Category { get; set; } = "Toàn bộ";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO cho tạo Voucher mới
    /// </summary>
    public class CreateVoucherDto
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string DiscountType { get; set; } = "percentage";
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscount { get; set; }
        public decimal MinOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public string Status { get; set; } = "active";
        public string Category { get; set; } = "Toàn bộ";
    }

    /// <summary>
    /// DTO cho cập nhật Voucher
    /// </summary>
    public class UpdateVoucherDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscount { get; set; }
        public decimal? MinOrderValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UsageLimit { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
    }
}
