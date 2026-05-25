# 🎯 HƯỚNG DẪN ĐĂNG NHẬP VÀ KIỂM TRA GIAO DIỆN ADMIN

## 📋 Tài Khoản Admin Mặc Định

```
📧 Email: admin@oishipan.com
🔐 Password: Admin@123
👤 Role: Admin
✅ Status: Active
```

**Lưu ý:** Tài khoản này được tạo **tự động** khi database khởi tạo lần đầu.

---

## 🚀 Cách Chạy Project

### 1. Mở Terminal 1 - Chạy API (Port 5000)
```bash
cd OishipanAPI
dotnet run
```
✅ Khi thấy dòng: `Now listening on: http://localhost:5000`

### 2. Mở Terminal 2 - Chạy MVC (Port 3000)
```bash
cd OishipanMVC
dotnet run
```
✅ Khi thấy dòng: `Now listening on: http://localhost:3000`

---

## 🌐 Bước 1: Truy Cập Trang Đăng Nhập

### Cách 1: Từ URL trực tiếp
```
http://localhost:3000/dang-nhap
```

### Cách 2: Từ Trang Chủ
1. Truy cập: `http://localhost:3000`
2. Click nút "Đăng Nhập" (hoặc "Login")

---

## ✅ Bước 2: Đăng Nhập Tài Khoản Admin

### Nhập Thông Tin:
```
Email: admin@oishipan.com
Password: Admin@123
```

### Nhấn Nút "Đăng Nhập"

**Kết quả kỳ vọng:**
- ✅ Đăng nhập thành công
- ✅ Chuyển hướng tới trang chủ hoặc dashboard
- ✅ Có tên người dùng hiển thị ở thanh navigation

---

## 🏛️ Bước 3: Truy Cập Giao Diện Admin

### Sau khi đăng nhập, truy cập Admin Panel:
```
http://localhost:3000/admin
```

### Hoặc:
1. Click vào tên tài khoản ở thanh navigation
2. Chọn "Admin Panel" hoặc "Quản Lý"

---

## 📊 Các Chức Năng Admin Để Kiểm Tra

### 1️⃣ Dashboard (Trang Chủ Admin)
```
URL: http://localhost:3000/admin
```
- Xem thống kê (Sản phẩm, Đơn hàng, Khách hàng)
- Xem các đơn hàng gần đây

### 2️⃣ Quản Lý Sản Phẩm
```
URL: http://localhost:3000/admin/products
```
**Các chức năng:**
- 📋 Xem danh sách sản phẩm
- ➕ Tạo sản phẩm mới
- ✏️ Chỉnh sửa sản phẩm
- 🗑️ Xóa sản phẩm
- 📸 Upload ảnh (Cloudinary)

### 3️⃣ Quản Lý Đơn Hàng
```
URL: http://localhost:3000/admin/orders
```
**Các chức năng:**
- 📊 Xem danh sách đơn hàng
- 👁️ Xem chi tiết đơn hàng
- 📝 Cập nhật trạng thái đơn hàng

### 4️⃣ Hồ Sơ Người Dùng
```
URL: http://localhost:3000/ho-so
```
- Xem thông tin cá nhân
- Cập nhật thông tin (Họ tên, SĐT, Địa chỉ)

---

## 🔍 Kiểm Tra API từ Swagger

### 1. Mở Swagger UI
```
http://localhost:5000/swagger
```

### 2. Kiểm Tra Đăng Nhập (POST)
1. Tìm mục **Auth** → Click **POST /api/auth/login**
2. Click "Try it out"
3. Nhập JSON:
```json
{
  "email": "admin@oishipan.com",
  "password": "Admin@123"
}
```
4. Click "Execute"
5. Kết quả kỳ vọng: `200 OK` với JWT token

**Phản hồi mẫu:**
```json
{
  "success": true,
  "message": "Đăng nhập thành công",
  "user": {
    "userId": 1,
    "fullName": "Oishipan Admin",
    "email": "admin@oishipan.com",
    "phoneNumber": "0123456789",
    "role": "Admin",
    "address": "Văn phòng Oishipan",
    "status": true
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 3. Kiểm Tra Đăng Ký (POST)
1. Tìm mục **Auth** → Click **POST /api/auth/register**
2. Click "Try it out"
3. Nhập JSON:
```json
{
  "fullName": "Test User",
  "email": "testuser@example.com",
  "phoneNumber": "0912345678",
  "password": "Test@123",
  "address": "123 Main Street"
}
```
4. Click "Execute"
5. Kết quả kỳ vọng: `200 OK`

---

## 🧪 Kiểm Tra API từ Postman (Tùy Chọn)

### 1. Setup Postman

**Import Collection:**
```
File → Import → Paste raw text hoặc URL
```

**Hoặc Manual Create:**

### 2. Kiểm Tra Login
```
Method: POST
URL: http://localhost:5000/api/auth/login
Headers: Content-Type: application/json
Body:
{
  "email": "admin@oishipan.com",
  "password": "Admin@123"
}
```

### 3. Lấy JWT Token từ Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 4. Sử Dụng Token cho API Protected
```
Headers:
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 🔐 Xác Thực JWT

### Cách JWT Hoạt Động:

1. **Đăng Nhập**
   - Client gửi: email + password
   - Server trả về: JWT Token
   - Client lưu token

2. **Truy Cập API Protected**
   - Client gửi: `Authorization: Bearer <token>`
   - Server xác minh token
   - Nếu hợp lệ → Cho phép truy cập

3. **Token Hết Hạn**
   - Token có thời hạn (thường 1 giờ)
   - Cần đăng nhập lại để lấy token mới

---

## 📝 Danh Sách API Endpoints

### Authentication
```
POST   /api/auth/login              → Đăng nhập
POST   /api/auth/register           → Đăng ký
GET    /api/auth/profile/{userId}   → Lấy hồ sơ
PUT    /api/auth/update-profile/{userId} → Cập nhật hồ sơ
```

### Products
```
GET    /api/products                → Lấy tất cả sản phẩm
GET    /api/products/{id}           → Lấy chi tiết sản phẩm
POST   /api/products                → Tạo sản phẩm (Admin)
PUT    /api/products/{id}           → Cập nhật sản phẩm (Admin)
DELETE /api/products/{id}           → Xóa sản phẩm (Admin)
```

### Upload
```
POST   /api/upload                  → Upload ảnh lên Cloudinary
```

### Orders
```
GET    /api/orders                  → Lấy đơn hàng
POST   /api/orders                  → Tạo đơn hàng
```

---

## 🆘 Khắc Phục Sự Cố

### ❌ Lỗi: "API Not Running"
**Giải pháp:**
1. Kiểm tra Terminal 1 (API) có chạy không
2. Mở: `http://localhost:5000/health`
3. Nếu không mở được → Chạy lại `dotnet run` ở Terminal 1

### ❌ Lỗi: "Đăng nhập thất bại"
**Giải pháp:**
1. Kiểm tra email/password đúng không
2. Kiểm tra API log có lỗi gì
3. Thử tại Swagger `/api/auth/login`

### ❌ Lỗi: "Cannot Access Admin Panel"
**Giải pháp:**
1. Đảm bảo đã đăng nhập
2. Kiểm tra role là "Admin"
3. Xóa cookies → Đăng nhập lại

### ❌ Lỗi: "Database Error"
**Giải pháp:**
1. Kiểm tra connection string trong `appsettings.json`
2. Đảm bảo SQL Server đang chạy
3. Run migrations: `dotnet ef database update`

---

## ✨ Chức Năng Để Kiểm Tra

### ✅ Authentication
- [x] Đăng nhập admin
- [x] Đăng ký người dùng mới
- [x] Đăng xuất
- [x] Xem hồ sơ cá nhân
- [x] Cập nhật thông tin cá nhân

### ✅ Products Management
- [x] Xem danh sách sản phẩm
- [x] Tạo sản phẩm mới (với upload ảnh)
- [x] Chỉnh sửa sản phẩm
- [x] Xóa sản phẩm
- [x] Lọc sản phẩm theo danh mục/thương hiệu

### ✅ Admin Panel
- [x] Truy cập dashboard
- [x] Xem thống kê
- [x] Quản lý sản phẩm
- [x] Quản lý đơn hàng
- [x] Quản lý người dùng (nếu có)

### ✅ API Documentation
- [x] Swagger UI hoạt động
- [x] Tất cả endpoints được documented
- [x] Có thể test API từ Swagger

---

## 📚 Tài Liệu Liên Quan

Xem thêm các file documentation:
- `COMPLETION_REPORT.md` - Báo cáo toàn bộ fixes
- `QUICK_START_TESTING.md` - Hướng dẫn testing
- `API_DOCUMENTATION.md` - Chi tiết các API endpoints
- `ARCHITECTURE_DIAGRAM.md` - Thiết kế hệ thống

---

## 🎓 Tóm Tắt

### Bước 1: Chạy Project
```bash
# Terminal 1
cd OishipanAPI && dotnet run

# Terminal 2
cd OishipanMVC && dotnet run
```

### Bước 2: Đăng Nhập Admin
```
URL: http://localhost:3000/dang-nhap
Email: admin@oishipan.com
Password: Admin@123
```

### Bước 3: Truy Cập Admin Panel
```
URL: http://localhost:3000/admin
```

### Bước 4: Kiểm Tra API (Tùy Chọn)
```
Swagger: http://localhost:5000/swagger
```

---

## 💡 Ghi Chú Quan Trọng

1. **Tài khoản Admin** được tạo **tự động** lần đầu
2. **JWT Token** được sử dụng cho API authentication
3. **Cookie** được sử dụng cho MVC session
4. **Cloudinary** được sử dụng để lưu trữ ảnh
5. **Role-based Authorization** được áp dụng cho Admin areas

---

**Status: ✅ SẴN SÀNG KIỂM TRA**

Nếu có bất kỳ vấn đề nào, hãy kiểm tra API logs hoặc browser console.
