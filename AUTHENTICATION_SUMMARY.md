# Hệ thống Xác thực và Phân quyền - Tóm tắt

## 1. Cấu trúc Hệ thống Xác thực

### API Layer (OishipanAPI)
- **Loại xác thực**: Session/Token-based (không dùng JWT)
- **Các DTOs đã được cập nhật**: 
  - `LoginRequest`: Email, Password (với validation)
  - `RegisterRequest`: FullName, Email, PhoneNumber, Password, Address (với validation)
  - `LoginResponse`: Success, Message, User (không chứa Token)
  - `UserDto`: Thông tin người dùng

### MVC Layer (OishipanMVC)
- **Loại xác thực**: Cookie-based
- **Cookie middleware**: Cấu hình trong Program.cs
- **Đường dẫn bảo mật**:
  - LoginPath: `/dang-nhap`
  - LogoutPath: `/dang-xuat`
  - AccessDeniedPath: `/truy-cap-bi-cam`

## 2. Validation Rules

### Login
- Email: Bắt buộc, định dạng email hợp lệ
- Password: Bắt buộc, tối thiểu 6 ký tự

### Register
- FullName: Bắt buộc, 3-100 ký tự
- Email: Bắt buộc, định dạng email hợp lệ
- PhoneNumber: Bắt buộc, bắt đầu bằng 0, 10 chữ số (0xxxxxxxxx)
- Password: Bắt buộc, 6-100 ký tự
- Address: Tùy chọn, tối đa 200 ký tự

### Xác thực từ phía Server
- **Email trùng**: Không cho phép đăng ký email đã tồn tại
- **Số điện thoại trùng**: Không cho phép đăng ký số điện thoại đã tồn tại
- **Trạng thái tài khoản**: Chỉ cho phép đăng nhập tài khoản có Status = true

## 3. Luồng Xác thực

### Đăng nhập (Login)
```
1. Người dùng nhập Email + Password
2. MVC validate định dạng
3. Gửi request tới API: POST /api/auth/login
4. API kiểm tra:
   - Email tồn tại
   - Password đúng
   - Tài khoản kích hoạt (Status = true)
5. Nếu thành công:
   - Trả về User object
   - MVC tạo Cookie authentication
   - Redirect tới Home
6. Nếu thất bại:
   - Trả về error message
   - Hiển thị thông báo lỗi trên Login form
```

### Đăng ký (Register)
```
1. Người dùng điền Form
2. MVC validate dữ liệu
3. Gửi request tới API: POST /api/auth/register
4. API kiểm tra:
   - Các trường bắt buộc
   - Email chưa tồn tại
   - Số điện thoại chưa tồn tại
5. Nếu thành công:
   - Tạo tài khoản mới (Role = "User", Status = true)
   - Redirect tới Login
6. Nếu thất bại:
   - Hiển thị error message
   - Giữ form data (ngoại trừ password)
```

## 4. Các Endpoint API

### Authentication Endpoints
- `POST /api/auth/login` - Đăng nhập
  - Request: { email, password }
  - Response: { success, message, user }
  
- `POST /api/auth/register` - Đăng ký
  - Request: { fullName, email, phoneNumber, password, address }
  - Response: { success, message }
  
- `GET /api/auth/profile/{userId}` - Lấy thông tin hồ sơ
  - Response: UserDto
  
- `PUT /api/auth/update-profile/{userId}` - Cập nhật hồ sơ
  - Request: { fullName, phoneNumber, address }
  - Response: { message }

### Orders Endpoints (dành cho người dùng đã đăng nhập)
- `POST /api/orders` - Tạo đơn hàng
- `GET /api/orders/{id}` - Lấy thông tin đơn hàng
- `GET /api/orders/user/{userId}/my-orders` - Lấy đơn hàng của người dùng
- `GET /api/orders` - Lấy tất cả đơn hàng (Admin)
- `PUT /api/orders/{id}/status` - Cập nhật trạng thái (Admin/Staff)
- `DELETE /api/orders/{id}/cancel` - Hủy đơn hàng

## 5. Các Trang MVC

### Public Pages
- `/dang-nhap` (GET/POST) - Đăng nhập
- `/dang-ky` (GET/POST) - Đăng ký
- `/truy-cap-bi-cam` (GET) - Trang từ chối truy cập

### Protected Pages (Cần đăng nhập - [Authorize])
- `/ho-so` (GET) - Xem hồ sơ cá nhân
- `/cap-nhat-ho-so` (POST) - Cập nhật hồ sơ
- `/dang-xuat` (GET) - Đăng xuất

## 6. Validation Frontend

### Login Form
- Real-time validation khi blur field
- Kiểm tra định dạng email
- Kiểm tra độ dài password
- Disabled submit button khi đang gửi request

### Register Form
- Real-time validation cho tất cả fields
- Validate trước khi submit
- Thông báo lỗi chi tiết
- Spinner khi đang gửi request

## 7. Xử lý Lỗi

### API Errors
```
Status 400: Bad Request (validation error)
Response: { success: false, message: "...", errors?: [...] }

Status 401: Unauthorized (login failed)
Response: { success: false, message: "Email hoặc mật khẩu không chính xác" }

Status 400: Bad Request (register failed)
Response: { success: false, message: "Email đã tồn tại" / "Số điện thoại đã tồn tại" }
```

### MVC Error Handling
- Try-catch blocks cho tất cả API calls
- Error messages hiển thị dưới dạng alerts
- Preserve form data (trừ password) khi có lỗi
- Redirect tới login nếu session hết hạn

## 8. Roles & Authorization

### User Roles
- **User**: Vai trò mặc định khi đăng ký
- **Admin**: Quản lý hệ thống (được tạo từ database seed)
- **Staff**: Nhân viên hỗ trợ (nếu cần thiết)

### Role-based Access
- **Public**: Xem sản phẩm, Đăng nhập, Đăng ký
- **User**: Tạo đơn hàng, Xem đơn hàng của mình, Cập nhật hồ sơ
- **Admin/Staff**: Quản lý đơn hàng, Cập nhật trạng thái

## 9. Bảo mật (Security)

- Mật khẩu được hash bằng BCrypt (work factor 12)
- Cookie authentication được set httpOnly
- Session timeout 1 giờ
- CSRF protection (ASP.NET Core MVC mặc định)
- Email/Phone uniqueness check
- Input validation trên cả Client và Server

## 10. Cách Sử Dụng

### Tạo tài khoản Admin mới
Database migration đã tạo tài khoản admin:
- Email: `admin@oishipan.com`
- Password: `Admin@123`
- Role: `Admin`

### Đăng nhập/Đăng ký người dùng
1. Vào `/dang-ky` để tạo tài khoản mới
2. Điền đầy đủ các trường (validate realtime)
3. Nếu thành công → redirect tới `/dang-nhap`
4. Nhập email + password → click "Đăng nhập"
5. Nếu thành công → redirect tới trang chủ (đã đăng nhập)

### Xem/Cập nhật hồ sơ
1. Khi đã đăng nhập, truy cập `/ho-so`
2. Xem thông tin: Email (read-only), ID, Full Name, Phone, Address
3. Chỉnh sửa Full Name/Phone/Address
4. Click "Cập nhật hồ sơ" để lưu thay đổi

### Đăng xuất
- Click "Đăng xuất" trên trang hồ sơ hoặc từ nav
- Cookie sẽ bị xóa
- Redirect tới trang chủ
