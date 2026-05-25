# 🧪 TEST CASES - KIỂM TRA HỆ THỐNG AUTHENTICATION

## 🎯 Mục Tiêu
Kiểm tra toàn bộ hệ thống authentication từ API đến MVC Frontend

---

## 📋 Danh Sách Test Cases

### TEST 1: API Health Check ✅
**Mục đích:** Kiểm tra API có chạy không

**Bước thực hiện:**
1. Mở browser
2. Truy cập: `http://localhost:5000/health`

**Kết quả kỳ vọng:**
```json
{
  "status": "healthy"
}
```

**Status code:** `200 OK`

---

### TEST 2: Login API - Thành Công ✅
**Mục đích:** Kiểm tra đăng nhập API với tài khoản admin

**Cách 1: Dùng Swagger**
```
URL: http://localhost:5000/swagger
Path: Auth → POST /api/auth/login
```

**Bước thực hiện:**
1. Click "Try it out"
2. Nhập body:
```json
{
  "email": "admin@oishipan.com",
  "password": "Admin@123"
}
```
3. Click "Execute"

**Kết quả kỳ vọng (200 OK):**
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

**Kiểm tra thêm:**
- [ ] `success` = true
- [ ] `token` không rỗng
- [ ] `user.role` = "Admin"
- [ ] `user.status` = true

---

### TEST 3: Login API - Sai Mật Khẩu ❌
**Mục đích:** Kiểm tra error handling khi mật khẩu sai

**Bước thực hiện:**
1. Dùng Swagger: `POST /api/auth/login`
2. Nhập:
```json
{
  "email": "admin@oishipan.com",
  "password": "WrongPassword"
}
```
3. Click "Execute"

**Kết quả kỳ vọng (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Email hoặc mật khẩu không chính xác hoặc tài khoản bị khóa"
}
```

**Kiểm tra:**
- [ ] Status code = 401
- [ ] `success` = false
- [ ] Có error message rõ ràng

---

### TEST 4: Login API - Email Không Tồn Tại ❌
**Mục đích:** Kiểm tra error handling khi email không tồn tại

**Bước thực hiện:**
1. Dùng Swagger: `POST /api/auth/login`
2. Nhập:
```json
{
  "email": "nonexistent@example.com",
  "password": "Test@123"
}
```
3. Click "Execute"

**Kết quả kỳ vọng (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Email hoặc mật khẩu không chính xác hoặc tài khoản bị khóa"
}
```

**Kiểm tra:**
- [ ] Status code = 401
- [ ] `success` = false

---

### TEST 5: Register API - Thành Công ✅
**Mục đích:** Kiểm tra tạo tài khoản mới

**Bước thực hiện:**
1. Dùng Swagger: `POST /api/auth/register`
2. Nhập:
```json
{
  "fullName": "Test User",
  "email": "testuser@example.com",
  "phoneNumber": "0987654321",
  "password": "Test@123",
  "address": "123 Main Street"
}
```
3. Click "Execute"

**Kết quả kỳ vọng (200 OK):**
```json
{
  "success": true,
  "message": "Đăng ký thành công. Vui lòng đăng nhập"
}
```

**Kiểm tra:**
- [ ] Status code = 200
- [ ] `success` = true
- [ ] User được lưu vào database

---

### TEST 6: Register API - Email Đã Tồn Tại ❌
**Mục đích:** Kiểm tra validation email duplicate

**Bước thực hiện:**
1. Dùng Swagger: `POST /api/auth/register`
2. Nhập (dùng email đã tồn tại):
```json
{
  "fullName": "Another User",
  "email": "testuser@example.com",
  "phoneNumber": "0111111111",
  "password": "Test@123",
  "address": "456 Test Ave"
}
```
3. Click "Execute"

**Kết quả kỳ vọng (400 Bad Request):**
```json
{
  "success": false,
  "message": "Email này đã được đăng ký"
}
```

**Kiểm tra:**
- [ ] Status code = 400
- [ ] `success` = false
- [ ] Error message chỉ ra email đã tồn tại

---

### TEST 7: Register API - Phone Đã Tồn Tại ❌
**Mục đích:** Kiểm tra validation số điện thoại duplicate

**Bước thực hiện:**
1. Dùng Swagger: `POST /api/auth/register`
2. Nhập (dùng phone đã tồn tại):
```json
{
  "fullName": "Another User",
  "email": "anotheruser@example.com",
  "phoneNumber": "0987654321",
  "password": "Test@123",
  "address": "789 Test Ln"
}
```
3. Click "Execute"

**Kết quả kỳ vọng (400 Bad Request):**
```json
{
  "success": false,
  "message": "Số điện thoại này đã được đăng ký"
}
```

---

### TEST 8: Get Profile API ✅
**Mục đích:** Kiểm tra lấy thông tin user profile

**Bước thực hiện:**
1. Từ TEST 2, copy JWT token
2. Dùng Swagger: `GET /api/auth/profile/{userId}`
3. Input userId = 1
4. Thêm header:
```
Authorization: Bearer [JWT_TOKEN_FROM_TEST_2]
```
5. Click "Execute"

**Kết quả kỳ vọng (200 OK):**
```json
{
  "userId": 1,
  "fullName": "Oishipan Admin",
  "email": "admin@oishipan.com",
  "phoneNumber": "0123456789",
  "role": "Admin",
  "address": "Văn phòng Oishipan",
  "status": true
}
```

---

### TEST 9: MVC Login - Thành Công ✅
**Mục đích:** Kiểm tra login từ giao diện MVC

**Bước thực hiện:**
1. Truy cập: `http://localhost:3000/dang-nhap`
2. Nhập:
   - Email: `admin@oishipan.com`
   - Password: `Admin@123`
3. Click "Đăng Nhập"

**Kết quả kỳ vọng:**
- [ ] Form được submit thành công
- [ ] Trang chuyển hướng tới home
- [ ] Tên "Oishipan Admin" hiển thị ở navbar
- [ ] Cookie được thiết lập

---

### TEST 10: MVC Register - Thành Công ✅
**Mục đích:** Kiểm tra đăng ký từ giao diện MVC

**Bước thực hiện:**
1. Truy cập: `http://localhost:3000/dang-ky`
2. Nhập:
   - Họ tên: `John Doe`
   - Email: `johndoe@example.com`
   - Số điện thoại: `0912345678`
   - Mật khẩu: `Test@123`
   - Địa chỉ: `123 Main Street`
3. Click "Đăng Ký"

**Kết quả kỳ vọng:**
- [ ] Form được submit thành công
- [ ] Thấy message "Đăng ký thành công"
- [ ] Trang chuyển hướng tới login
- [ ] Có thể đăng nhập bằng tài khoản vừa tạo

---

### TEST 11: MVC Login với Tài Khoản Vừa Tạo ✅
**Mục đích:** Kiểm tra login tài khoản mới đăng ký

**Bước thực hiện:**
1. Truy cập: `http://localhost:3000/dang-nhap`
2. Nhập:
   - Email: `johndoe@example.com`
   - Password: `Test@123`
3. Click "Đăng Nhập"

**Kết quả kỳ vọng:**
- [ ] Login thành công
- [ ] Tên "John Doe" hiển thị ở navbar
- [ ] Trang chuyển hướng tới home

---

### TEST 12: Access Admin Panel - Thành Công ✅
**Mục đích:** Kiểm tra admin có thể truy cập admin panel

**Bước thực hiện:**
1. Đảm bảo đã login là admin
2. Truy cập: `http://localhost:3000/admin`

**Kết quả kỳ vọng:**
- [ ] Trang admin dashboard tải thành công
- [ ] Hiển thị thống kê
- [ ] Có các menu quản lý sản phẩm, đơn hàng, etc.

---

### TEST 13: Access Admin Panel - User Thường ❌
**Mục đích:** Kiểm tra user thường không thể truy cập admin

**Bước thực hiện:**
1. Logout (nếu đang login admin)
2. Login với tài khoản regular user (johndoe@example.com)
3. Truy cập: `http://localhost:3000/admin`

**Kết quả kỳ vọng:**
- [ ] Bị chuyển hướng tới access denied page
- [ ] Hoặc thấy message "Không có quyền truy cập"
- [ ] Không thể vào admin panel

---

### TEST 14: Logout - Thành Công ✅
**Mục đích:** Kiểm tra logout

**Bước thực hiện:**
1. Đảm bảo đã login
2. Click nút "Đăng Xuất" ở navbar
3. Hoặc truy cập: `http://localhost:3000/dang-xuat`

**Kết quả kỳ vọng:**
- [ ] Logout thành công
- [ ] Cookie bị xóa
- [ ] Tên người dùng biến mất từ navbar
- [ ] Chuyển hướng tới home
- [ ] Không thể truy cập protected pages

---

### TEST 15: Profile Page - View ✅
**Mục đích:** Kiểm tra xem thông tin cá nhân

**Bước thực hiện:**
1. Login với tài khoản admin
2. Truy cập: `http://localhost:3000/ho-so`
3. Hoặc click vào "Hồ Sơ" ở navbar

**Kết quả kỳ vọng:**
- [ ] Trang profile tải thành công
- [ ] Hiển thị thông tin:
  - Họ tên: "Oishipan Admin"
  - Email: "admin@oishipan.com"
  - SĐT: "0123456789"
  - Địa chỉ: "Văn phòng Oishipan"

---

### TEST 16: Profile Page - Update ✅
**Mục đích:** Kiểm tra cập nhật thông tin cá nhân

**Bước thực hiện:**
1. Ở trang profile
2. Thay đổi thông tin:
   - Họ tên: "Admin Updated"
   - SĐT: "0999999999"
   - Địa chỉ: "New Address"
3. Click "Cập Nhật"

**Kết quả kỳ vọng:**
- [ ] Cập nhật thành công
- [ ] Thấy message "Cập nhật thành công"
- [ ] Thông tin được lưu vào database

---

### TEST 17: Validation - Empty Fields ❌
**Mục đích:** Kiểm tra validation khi bỏ trống fields

**Bước thực hiện:**
1. Truy cập: `http://localhost:3000/dang-nhap`
2. Bỏ trống email
3. Click "Đăng Nhập"

**Kết quả kỳ vọng:**
- [ ] Client-side validation hiển thị error
- [ ] Hoặc server-side error message
- [ ] Message: "Email không được để trống"

---

### TEST 18: Validation - Invalid Email ❌
**Mục đích:** Kiểm tra validation email format

**Bước thực hiện:**
1. Truy cập: `http://localhost:3000/dang-ky`
2. Nhập:
   - Email: `invalidemail` (không có @)
3. Click "Đăng Ký"

**Kết quả kỳ vọng:**
- [ ] Validation error
- [ ] Message: "Email không hợp lệ"

---

### TEST 19: Validation - Short Password ❌
**Mục đích:** Kiểm tra validation độ dài mật khẩu

**Bước thực hiện:**
1. Truy cập: `http://localhost:3000/dang-ky`
2. Nhập:
   - Mật khẩu: `123` (dưới 6 ký tự)
3. Click "Đăng Ký"

**Kết quả kỳ vọng:**
- [ ] Validation error
- [ ] Message: "Mật khẩu phải từ 6 ký tự trở lên"

---

### TEST 20: Validation - Invalid Phone ❌
**Mục đích:** Kiểm tra validation số điện thoại

**Bước thực hiện:**
1. Truy cập: `http://localhost:3000/dang-ky`
2. Nhập:
   - SĐT: `123456` (không đúng format)
3. Click "Đăng Ký"

**Kết quả kỳ vọng:**
- [ ] Validation error
- [ ] Message: "Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số"

---

## 📊 Test Summary Template

```
TEST CASE: [Test Name]
═══════════════════════════════════════════════════════════

✓ Precondition: [Setup required]
✓ Steps: [Test steps]
✓ Expected Result: [What should happen]
✓ Actual Result: [What actually happened]
✓ Status: [PASS / FAIL]
✓ Notes: [Any issues or observations]
```

---

## 🎯 Critical Test Cases

These tests MUST pass before going to production:

- [ ] TEST 2: Login API - Thành Công
- [ ] TEST 5: Register API - Thành Công
- [ ] TEST 9: MVC Login - Thành Công
- [ ] TEST 10: MVC Register - Thành Công
- [ ] TEST 12: Access Admin Panel - Thành Công
- [ ] TEST 13: Access Admin Panel - User Thường (FAIL expected)
- [ ] TEST 14: Logout - Thành Công

---

## 📝 Test Execution Log

| Test# | Name | Expected | Actual | Status | Date | Notes |
|-------|------|----------|--------|--------|------|-------|
| 1 | Health Check | 200 OK | | | | |
| 2 | Login API Success | 200 OK + Token | | | | |
| 3 | Login API Wrong Pwd | 401 Error | | | | |
| 4 | Login API No Email | 401 Error | | | | |
| 5 | Register API Success | 200 OK | | | | |
| 6 | Register API Dup Email | 400 Error | | | | |
| 7 | Register API Dup Phone | 400 Error | | | | |
| 8 | Get Profile API | 200 OK | | | | |
| 9 | MVC Login Success | Redirect + Cookie | | | | |
| 10 | MVC Register Success | Success Message | | | | |
| 11 | MVC Login New User | Redirect + Cookie | | | | |
| 12 | Admin Access Success | Admin Panel | | | | |
| 13 | Admin Access Denied | Access Denied | | | | |
| 14 | Logout Success | Redirect | | | | |
| 15 | Profile View | Show Data | | | | |
| 16 | Profile Update | Success Msg | | | | |
| 17 | Empty Fields | Error Message | | | | |
| 18 | Invalid Email | Error Message | | | | |
| 19 | Short Password | Error Message | | | | |
| 20 | Invalid Phone | Error Message | | | | |

---

## ✅ Sign Off

Khi tất cả tests passed, hãy ký dưới:

```
Tested by: _______________
Date: _____________________
Status: __________________
Notes: ____________________
```

---

**Total Test Cases: 20**
**Category: Authentication & Authorization**
**Environment: Development**
**Status: Ready for Testing**
