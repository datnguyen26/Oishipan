# 🔧 Backend API Changes - Voucher Management

## 📝 Tóm Tắt Thay Đổi

Các thay đổi được thực hiện trên OishipanAPI để hỗ trợ hệ thống quản lý voucher đầy đủ.

## ✅ Models (OishipanAPI/Models/)

### Voucher.cs - CẬP NHẬT

```csharp
public class Voucher
{
    [Key]
    public int VoucherId { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string Description { get; set; } = null!;

    // percentage hoặc fixed
    [Required]
    [StringLength(20)]
    public string DiscountType { get; set; } = "percentage";

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountValue { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MaxDiscount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinOrderValue { get; set; } = 0;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public int UsageLimit { get; set; }

    [Required]
    public int UsageCount { get; set; } = 0;

    // active, scheduled, expired
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "active";

    [StringLength(100)]
    public string Category { get; set; } = "Toàn bộ";

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
```

## 📦 DTOs (OishipanAPI/DTOs/)

### VoucherDto.cs - MỚI TẠO

```csharp
public class VoucherDto
{
    public int VoucherId { get; set; }
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
    public int UsageCount { get; set; }
    public string Status { get; set; } = "active";
    public string Category { get; set; } = "Toàn bộ";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

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
```

## 🔧 Services (OishipanAPI/Services/)

### IServiceInterfaces.cs - CẬP NHẬT

```csharp
public interface IVoucherService
{
    Task<VoucherDto> GetVoucherByCodeAsync(string code);
    Task<VoucherDto> GetVoucherByIdAsync(int id);
    Task<List<VoucherDto>> GetAllVouchersAsync();
    Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto);
    Task<VoucherDto> UpdateVoucherAsync(int id, UpdateVoucherDto dto);
    Task<bool> DeleteVoucherAsync(int voucherId);
    Task<bool> IsVoucherValidAsync(string code);
}
```

### VoucherService.cs - CẬP NHẬT HOÀN TOÀN

- Thêm phương thức `GetVoucherByIdAsync()`
- Thêm phương thức `UpdateVoucherAsync()` với logic update toàn bộ fields
- Cải thiện `IsVoucherValidAsync()` để kiểm tra status, ngày hiệu lực, và hạn mức
- Thêm validation duplicate code trước khi tạo

## 🌐 Controllers (OishipanAPI/Controllers/)

### VouchersController.cs - CẬP NHẬT HOÀN TOÀN

**Public Endpoints:**
```
GET /api/vouchers/{code}              - Lấy voucher theo mã
GET /api/vouchers/{code}/validate     - Kiểm tra tính hợp lệ
```

**Admin Only Endpoints:**
```
GET    /api/vouchers                   - Lấy danh sách tất cả
GET    /api/vouchers/id/{id}           - Lấy chi tiết theo ID
POST   /api/vouchers                   - Tạo mới
PUT    /api/vouchers/{id}              - Cập nhật
DELETE /api/vouchers/{id}              - Xóa
```

- Tất cả endpoints admin đều có `[Authorize(Roles = "Admin")]`
- Error handling tốt hơn với try-catch
- Response types được định nghĩa rõ ràng

## 📊 Database Migration

Cần chạy migration để cập nhật bảng Vouchers:

```bash
cd OishipanAPI
dotnet ef migrations add UpdateVoucherSchema
dotnet ef database update
```

**Hoặc chạy SQL thủ công:**

```sql
ALTER TABLE Vouchers ADD 
    Name NVARCHAR(200) NOT NULL DEFAULT '',
    Description NVARCHAR(500),
    DiscountType NVARCHAR(20) NOT NULL DEFAULT 'percentage',
    MaxDiscount DECIMAL(18,2) NOT NULL DEFAULT 0,
    MinOrderValue DECIMAL(18,2) NOT NULL DEFAULT 0,
    StartDate DATETIME NOT NULL DEFAULT GETDATE(),
    EndDate DATETIME NOT NULL DEFAULT GETDATE(),
    UsageLimit INT NOT NULL DEFAULT 100,
    UsageCount INT NOT NULL DEFAULT 0,
    Status NVARCHAR(20) NOT NULL DEFAULT 'active',
    Category NVARCHAR(100) DEFAULT 'Toàn bộ',
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME;

-- Drop old column nếu cần
-- ALTER TABLE Vouchers DROP COLUMN ExpiryDate;
```

## 🔐 Authorization

- Tất cả endpoints CRUD (POST, PUT, DELETE) yêu cầu role `Admin`
- GET endpoints công khai (public) không cần authorization
- GET danh sách tất cả yêu cầu authorization

## 📱 Response Format

### Success Response (200, 201)

```json
{
  "voucherId": 1,
  "code": "OISHI100",
  "name": "Voucher Example",
  "description": "Description...",
  "discountType": "percentage",
  "discountValue": 15,
  "maxDiscount": 50000,
  "minOrderValue": 100000,
  "startDate": "2026-06-01T00:00:00",
  "endDate": "2026-06-30T00:00:00",
  "usageLimit": 500,
  "usageCount": 142,
  "status": "active",
  "category": "Bánh Mì",
  "createdAt": "2026-06-01T10:30:00",
  "updatedAt": null
}
```

### Error Response (400, 401, 404)

```json
{
  "message": "Error description"
}
```

## 🧪 Testing API dengan Swagger

1. Chạy OishipanAPI: `dotnet run`
2. Mở Swagger: `http://localhost:5000/swagger`
3. Đăng nhập (POST /api/auth/login) để lấy token
4. Copy token vào Authorization header
5. Test các endpoints Voucher

## 🔄 CORS Configuration

Đảm bảo OishipanAPI/Program.cs có CORS policy cho React app:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

app.UseCors("AllowReact");
```

## 📋 Checklist Hoàn Thành

- ✅ Cập nhật Voucher model
- ✅ Tạo VoucherDto classes
- ✅ Cập nhật VoucherService
- ✅ Cập nhật IVoucherService interface
- ✅ Cập nhật VouchersController
- ✅ Thêm authorization
- ✅ Thêm error handling
- ⏳ Chạy migration database
- ⏳ Test endpoints với Swagger
- ⏳ Test từ React app

## 🔗 Liên Kết

- Backend: [OishipanAPI](../OishipanAPI/)
- Frontend: [admin-voucher](../../OishipanMVC/wwwroot/admin-voucher/)

---

**Ngày Cập Nhật**: 03/06/2026
