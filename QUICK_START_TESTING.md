# 🚀 Quick Start Testing Guide

## Prerequisites
1. Both API and MVC projects are running
2. SQL Server database is configured and running
3. Cloudinary account credentials are in `appsettings.json`

---

## Step 1: Start the Projects

### Terminal 1 - Start API (Port 5000)
```bash
cd OishipanAPI
dotnet run
# Output: Now listening on: http://localhost:5000
```

### Terminal 2 - Start MVC (Port 3000)
```bash
cd OishipanMVC
dotnet run
# Output: Now listening on: http://localhost:3000
```

---

## Step 2: Access Swagger API Documentation

### Open in Browser
```
http://localhost:5000/swagger
```

You should see the Swagger UI with all available endpoints:
- ✅ Auth endpoints (login, register, profile)
- ✅ Products endpoints (CRUD operations)
- ✅ Upload endpoint (image upload)
- ✅ Orders endpoints
- ✅ Vouchers endpoints

---

## Step 3: Test Login/Registration

### Option A: Using Swagger UI
1. Go to `http://localhost:5000/swagger`
2. Find "Auth" section
3. Click on `POST /api/auth/register`
4. Click "Try it out"
5. Enter sample data:
```json
{
  "fullName": "Test User",
  "email": "test@example.com",
  "phoneNumber": "0123456789",
  "password": "Test@123",
  "address": "123 Main St"
}
```
6. Click "Execute"
7. Should return `200 OK` with message "Registration successful"

### Option B: Using MVC Web Interface
1. Go to `http://localhost:3000/dang-ky`
2. Fill in registration form:
   - Full Name: Test User
   - Email: test@example.com
   - Phone: 0123456789
   - Password: Test@123
   - Address: 123 Main St
3. Click "Đăng Ký" (Register)
4. Should see success message

### Login Test
1. Go to `http://localhost:3000/dang-nhap`
2. Use credentials:
   - Email: admin@oishipan.com (already created in DB)
   - Password: Admin@123
3. Should redirect to home page

---

## Step 4: Test Cloudinary Image Upload

### Through Admin Panel
1. Login as Admin: `http://localhost:3000/dang-nhap`
   - Email: `admin@oishipan.com`
   - Password: `Admin@123`

2. Go to Admin Panel: `http://localhost:3000/admin`

3. Create New Product: `http://localhost:3000/admin/products/create`

4. Fill in form:
   - Product Name: Test Product
   - Price: 100000
   - Category ID: 1
   - Brand ID: 1
   - Description: Test description
   - Choose image file

5. Click "Tạo Sản Phẩm" (Create Product)

6. Image should be uploaded to Cloudinary and product created

### Through Swagger
1. Go to `http://localhost:5000/swagger`
2. Find "Upload" section
3. Click `POST /api/upload`
4. Click "Try it out"
5. Select an image file
6. Click "Execute"
7. Should return URL like:
```json
{
  "success": true,
  "message": "Image uploaded successfully",
  "url": "https://res.cloudinary.com/dghzmhqtb/image/upload/..."
}
```

---

## Step 5: Test Authorization & Roles

### Test Admin-Only Access
1. **Without Login**: Try `http://localhost:3000/admin`
   - Should redirect to login page

2. **As Regular User**: 
   - Login with user account
   - Try `http://localhost:3000/admin`
   - Should show "Access Denied"

3. **As Admin**:
   - Login as `admin@oishipan.com`
   - Go to `http://localhost:3000/admin`
   - Should see admin dashboard with full access

---

## Step 6: Verify API Endpoints

### Health Check
```bash
curl http://localhost:5000/health
# Response: {"status":"healthy"}
```

### List All Products
```bash
curl http://localhost:5000/api/products
# Response: Array of product objects
```

### Login Test
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@oishipan.com",
    "password": "Admin@123"
  }'
# Response: JWT token and user info
```

---

## Default Test Accounts

### Admin Account
- Email: `admin@oishipan.com`
- Password: `Admin@123`
- Role: Admin
- Full Access to admin panel

### How to Create User Account
1. Register at: `http://localhost:3000/dang-ky`
2. Login with the registered credentials
3. User account will have limited access (User role)

---

## Common Issues & Solutions

### Issue: "Cannot connect to API"
**Solution:**
1. Ensure API is running on port 5000
2. Check Windows Firewall allows localhost connections
3. Verify `appsettings.json` has correct API URL

### Issue: "Swagger not loading"
**Solution:**
1. Clear browser cache (Ctrl+Shift+Delete)
2. Try incognito/private mode
3. Check API is running: `http://localhost:5000/health`

### Issue: "Image upload fails"
**Solution:**
1. Check file size (max 5MB)
2. Check file type (jpg, png, gif, webp only)
3. Verify Cloudinary credentials in appsettings.json
4. Check API logs for detailed error

### Issue: "Cannot access admin panel"
**Solution:**
1. Ensure you're logged in
2. Login with admin account (admin@oishipan.com)
3. Clear cookies if previously logged in as regular user
4. Check user role in database

---

## Testing Checklist

- [ ] API is running on port 5000
- [ ] MVC is running on port 3000
- [ ] Swagger UI loads at `/swagger`
- [ ] Can view all endpoints in Swagger
- [ ] Can register new user account
- [ ] Can login with admin account
- [ ] Can create new product with image upload
- [ ] Image is stored in Cloudinary
- [ ] Can access admin panel as admin user
- [ ] Cannot access admin panel as regular user
- [ ] Health check endpoint responds

---

## Next Steps

After verifying all endpoints work:
1. Test additional user flows
2. Check database for created records
3. Verify Cloudinary has uploaded images
4. Test logout functionality
5. Test profile update functionality

**Good Luck! 🎉**
