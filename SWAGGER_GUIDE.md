# Hướng dẫn Swagger API - Oishipan

## 🚀 Cách Truy Cập Swagger UI

Khi server API chạy (development mode), bạn có thể truy cập Swagger UI tại:

```
http://localhost:5000/swagger
```

## 📋 Các API Endpoints Chính

### 🔐 Authentication (Auth)
- **POST** `/api/auth/login` - Đăng nhập
- **POST** `/api/auth/register` - Đăng ký tài khoản mới
- **GET** `/api/auth/profile/{userId}` - Lấy thông tin hồ sơ
- **PUT** `/api/auth/update-profile/{userId}` - Cập nhật hồ sơ

### 🍰 Products (Sản Phẩm)
- **GET** `/api/products` - Lấy danh sách tất cả sản phẩm
- **GET** `/api/products/{id}` - Lấy chi tiết sản phẩm
- **GET** `/api/products/category/{categoryId}` - Lấy sản phẩm theo danh mục
- **GET** `/api/products/brand/{brandId}` - Lấy sản phẩm theo thương hiệu
- **POST** `/api/products` - Tạo sản phẩm mới (Admin/Staff)
- **PUT** `/api/products/{id}` - Cập nhật sản phẩm (Admin/Staff)
- **DELETE** `/api/products/{id}` - Xóa sản phẩm (Admin/Staff)
- **POST** `/api/products/{id}/upload-image` - Tải ảnh sản phẩm (Admin/Staff)

### 📦 Orders (Đơn Hàng)
- **POST** `/api/orders` - Tạo đơn hàng mới
- **GET** `/api/orders/{id}` - Lấy chi tiết đơn hàng
- **GET** `/api/orders` - Lấy tất cả đơn hàng
- **GET** `/api/orders/user/{userId}/my-orders` - Lấy đơn hàng của người dùng
- **PUT** `/api/orders/{id}/status` - Cập nhật trạng thái đơn hàng
- **DELETE** `/api/orders/{id}/cancel` - Hủy đơn hàng

### 🎟️ Vouchers (Mã Khuyến Mại)
- **GET** `/api/vouchers/{code}` - Lấy voucher theo mã
- **GET** `/api/vouchers/{code}/validate` - Kiểm tra tính hợp lệ
- **GET** `/api/vouchers` - Lấy tất cả voucher (Admin)
- **POST** `/api/vouchers` - Tạo voucher mới (Admin)
- **DELETE** `/api/vouchers/{id}` - Xóa voucher (Admin)

## 🔑 JWT Authentication

Để sử dụng các API cần xác thực:

1. **Đăng nhập** qua `/api/auth/login` để lấy JWT Token
2. Trong Swagger UI, nhấn nút **"Authorize"** ở góc trên phải
3. Nhập: `Bearer {token}` (ví dụ: `Bearer eyJhbGc...`)
4. Nhấn **"Authorize"** để lưu token

Sau đó các API cần authentication sẽ được phép truy cập.

## 🌐 API Base URL

- **Development**: `http://localhost:5000`
- **Production**: Sẽ được cập nhật khi deploy

## 📝 Ví Dụ Request/Response

### Login
**Request:**
```json
POST /api/auth/login
{
  "email": "admin@oishipan.com",
  "password": "Admin@123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Create Product (Admin Only)
**Request:**
```json
POST /api/products
{
  "name": "Bánh mì hoa quả",
  "description": "Bánh mì tươi với hoa quả tây",
  "price": 150000,
  "categoryId": 1,
  "brandId": 1,
  "quantity": 50
}
```

**Response:**
```json
{
  "productId": 1,
  "name": "Bánh mì hoa quả",
  "description": "Bánh mì tươi với hoa quả tây",
  "price": 150000,
  "categoryId": 1,
  "brandId": 1,
  "quantity": 50,
  "image": null
}
```

## 🛠️ Troubleshooting

- **API không phản hồi**: Kiểm tra server API đang chạy (`dotnet run` trong thư mục OishipanAPI)
- **Token không hợp lệ**: Đăng nhập lại để lấy token mới
- **403 Forbidden**: Kiểm tra quyền (Admin/Staff) trong hệ thống
- **Swagger không hiển thị**: Làm mới trang, xóa cache browser, hoặc restart server

## 📚 XML Comments

Tất cả các API endpoint đều có XML comments với:
- Mô tả chi tiết về chức năng
- Thông tin tham số
- Các response codes (200, 400, 404, etc.)
- Ví dụ sử dụng

Xem chi tiết trong Swagger UI để hiểu rõ từng API.

## ✅ Danh Sách Packages Đã Cài

- **Microsoft.AspNetCore.OpenApi** 10.0.0
- **Microsoft.OpenApi** 2.0.0
- **Swashbuckle.AspNetCore** 6.9.0
- **Microsoft.EntityFrameworkCore** 10.0.0
- **Microsoft.AspNetCore.Authentication.JwtBearer** 10.0.0
- **CloudinaryDotNet** 1.27.0
- **BCrypt.Net-Next** 4.0.3

---

**Cập nhật lần cuối**: 2024
**Phiên bản API**: v1.0
