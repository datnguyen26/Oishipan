# 👤 ADMIN ACCOUNT CREDENTIALS

## 🎯 Official Admin Account

```
╔═══════════════════════════════════════════════════╗
║         OISHIPAN ADMIN LOGIN CREDENTIALS          ║
╠═══════════════════════════════════════════════════╣
║                                                   ║
║  📧 Email:      admin@oishipan.com               ║
║  🔐 Password:   Admin@123                        ║
║  👤 Full Name:  Oishipan Admin                   ║
║  📞 Phone:      0123456789                       ║
║  📍 Address:    Văn phòng Oishipan               ║
║  🔑 Role:       Admin (Full Access)              ║
║  ✅ Status:     Active                           ║
║                                                   ║
╚═══════════════════════════════════════════════════╝
```

---

## ⚙️ Khi Nào Tài Khoản Được Tạo?

**Admin account được tạo tự động:**
- ✅ Lần đầu tiên API chạy (`dotnet run`)
- ✅ Sau khi database migration hoàn tất
- ✅ **CHỈ NẾU** chưa có admin account nào trong database

**Code tại:** `OishipanAPI/Program.cs` (dòng ~120)

```csharp
if (!context.Accounts.Any(a => a.Role == "Admin"))
{
    context.Accounts.Add(new Account
    {
        FullName = "Oishipan Admin",
        Email = "admin@oishipan.com",
        PhoneNumber = "0123456789",
        Password = PasswordHelper.HashPassword("Admin@123"),
        Role = "Admin",
        Address = "Văn phòng Oishipan",
        Status = true
    });
    
    context.SaveChanges();
}
```

---

## 🚀 Cách Sử Dụng

### 1. Đăng Nhập Web MVC
```
URL: http://localhost:3000/dang-nhap
Email: admin@oishipan.com
Password: Admin@123
```

### 2. Kiểm Tra API
```
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@oishipan.com",
  "password": "Admin@123"
}
```

### 3. Swagger Testing
```
URL: http://localhost:5000/swagger
Tìm: Auth → POST /api/auth/login
Input: Email & Password ở trên
```

---

## 📊 Admin Permissions

**Tài khoản admin có quyền:**

### ✅ Quản Lý Sản Phẩm
- [x] Xem danh sách sản phẩm
- [x] Tạo sản phẩm mới
- [x] Chỉnh sửa sản phẩm
- [x] Xóa sản phẩm
- [x] Upload ảnh

### ✅ Quản Lý Đơn Hàng
- [x] Xem danh sách đơn hàng
- [x] Xem chi tiết đơn hàng
- [x] Cập nhật trạng thái

### ✅ Quản Lý Người Dùng
- [x] Xem danh sách người dùng
- [x] Xem chi tiết người dùng
- [x] Quản lý quyền hạn

### ✅ Truy Cập Admin Areas
- [x] `/admin` - Dashboard
- [x] `/admin/products` - Quản lý sản phẩm
- [x] `/admin/orders` - Quản lý đơn hàng
- [x] `/admin/users` - Quản lý người dùng

### ❌ Các quyền không có (dành cho tương lai)
- [ ] Quản lý báo cáo tài chính
- [ ] Quản lý nhân viên
- [ ] Quản lý hệ thống

---

## 🔐 Bảo Mật

### Password Hashing
```
Password: Admin@123
Hashed:  [bcrypt hash - không lưu plain text]
```

### Token Management
```
JWT Token được tạo sau khi login
Token có hiệu lực: 1 giờ
Token được lưu: Browser session
```

### Access Control
```
Role: Admin
Scope: Tất cả admin pages
Access Level: Full
```

---

## 🧪 Quick Test

### Test 1: Login Success
```bash
# Thay {API_PORT} bằng port thực tế (mặc định: 5000)
curl -X POST http://localhost:{API_PORT}/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@oishipan.com",
    "password": "Admin@123"
  }'
```

**Response:**
```json
{
  "success": true,
  "user": { ... },
  "token": "eyJ..."
}
```

### Test 2: Login Failed (Wrong Password)
```bash
# Thay {API_PORT} bằng port thực tế (mặc định: 5000)
curl -X POST http://localhost:{API_PORT}/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@oishipan.com",
    "password": "WrongPassword"
  }'
```

**Response:**
```json
{
  "success": false,
  "message": "Email hoặc mật khẩu không chính xác hoặc tài khoản bị khóa"
}
```

---

## 💡 Lưu Ý Quan Trọng

### ⚠️ Bảo Mật
- ❌ **KHÔNG** chia sẻ password trên chat/email
- ❌ **KHÔNG** commit password lên git
- ❌ **KHÔNG** thay đổi password trong code
- ✅ **NÊN** thay đổi password sau lần đăng nhập đầu tiên (tính năng sắp có)

### ⚠️ Development vs Production
- **Development:** Dùng tài khoản này để test
- **Production:** Phải thay đổi:
  - Password mạnh hơn
  - Email chính thức
  - Tất cả thông tin chính xác

### ⚠️ Database Reset
Nếu xóa database:
```bash
dotnet ef database drop --force
dotnet ef database update
```
Admin account sẽ được tạo lại tự động

---

## 📋 Multiple Admin Accounts (Future)

**Hiện tại:** Chỉ tạo 1 admin account tự động

**Để tạo thêm admin:**
1. Login với tài khoản hiện tại
2. Vào Admin Panel → Quản Lý Người Dùng
3. Tạo tài khoản mới với role = "Admin"

---

## 🔄 Change Password (Future Feature)

**Sắp có tính năng:**
- Đổi mật khẩu
- Reset mật khẩu qua email
- Two-factor authentication

**Hiện tại:**
- Để thay đổi password, sửa trong database hoặc code

---

## 📞 Troubleshooting

### ❌ "Email or password incorrect"
- [ ] Kiểm tra email chính xác: `admin@oishipan.com`
- [ ] Kiểm tra password: `Admin@123`
- [ ] Kiểm tra status = true (active)

### ❌ "Account not found"
- [ ] Kiểm tra admin account đã được tạo chưa
- [ ] Chạy API một lần để tạo admin
- [ ] Kiểm tra database

### ❌ "Invalid token"
- [ ] Token có thể đã hết hạn
- [ ] Đăng nhập lại để lấy token mới

### ❌ "Cannot access admin"
- [ ] Kiểm tra role = "Admin"
- [ ] Kiểm tra status = true
- [ ] Xóa cookies và đăng nhập lại

---

## 📊 Account Details in Database

**SQL Query để xem account:**
```sql
SELECT * FROM Accounts WHERE Email = 'admin@oishipan.com';
```

**Fields:**
```
UserId:     1
FullName:   Oishipan Admin
Email:      admin@oishipan.com
PhoneNumber: 0123456789
Password:   [hashed password]
Role:       Admin
Address:    Văn phòng Oishipan
Status:     1 (true/active)
```

---

## ✅ Verification Checklist

Sau khi login, kiểm tra:

- [ ] Tên "Oishipan Admin" hiển thị ở navbar
- [ ] Có thể truy cập `/admin` dashboard
- [ ] Menu quản lý sản phẩm, đơn hàng có sẵn
- [ ] JWT token được tạo trong background
- [ ] Cookie được thiết lập
- [ ] Có thể logout
- [ ] Sau logout, không thể truy cập `/admin`

---

## 🎯 Next Steps

1. ✅ **Ghi nhớ** credentials
2. ✅ **Chạy** project
3. ✅ **Đăng nhập** với admin account
4. ✅ **Kiểm tra** admin panel
5. ✅ **Tạo** test accounts
6. ✅ **Chạy** test cases

---

## 📚 Related Documentation

- `START_HERE.md` - Quick start guide
- `ADMIN_LOGIN_GUIDE.md` - Detailed login instructions
- `API_AUTHENTICATION_COMPLETE.md` - Technical details
- `TEST_CASES_AUTH.md` - 20 test cases

---

```
╔═════════════════════════════════════════════════════╗
║  ADMIN CREDENTIALS READY                            ║
║  ✅ Email: admin@oishipan.com                       ║
║  ✅ Password: Admin@123                             ║
║  ✅ Role: Admin (Full Access)                       ║
║  ✅ Status: Active                                  ║
╚═════════════════════════════════════════════════════╝
```

**Sẵn sàng để kiểm tra giao diện admin! 🚀**
