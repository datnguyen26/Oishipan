# 🚀 RUN PROJECT GUIDE

## ⚡ Cách Chạy Project Oishipan

### Bước 1: Chuẩn Bị

Đảm bảo bạn đã cài:
- ✅ .NET 10 SDK
- ✅ SQL Server (hoặc LocalDB)
- ✅ Visual Studio hoặc VS Code

### Bước 2: Update Database

```bash
cd OishipanAPI
dotnet ef database update
```

Nếu chưa có migration, chạy:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Bước 3: Chạy API Server

```bash
cd OishipanAPI
dotnet run
```

Output sẽ hiện:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to stop.
```

### Bước 4: Chạy MVC Project (Terminal khác)

```bash
cd OishipanMVC
dotnet run
```

MVC sẽ chạy tại: `http://localhost:5001`

### Bước 5: Truy Cập Swagger

Mở browser vào:
```
http://localhost:5000/swagger
```

---

## 🔐 Default Login Credentials

**Email**: `admin@oishipan.com`
**Password**: `Admin@123`

Tài khoản này được tạo tự động khi database initialize.

---

## 🧪 Test API Endpoints

### 1. Login
```
POST /api/auth/login
Body: {
  "email": "admin@oishipan.com",
  "password": "Admin@123"
}
```

Copy token từ response.

### 2. Authorize Swagger UI
1. Nhấn nút **Authorize** ở trên cùng
2. Paste: `Bearer {token}`
3. Nhấn Authorize

### 3. Get Products
```
GET /api/products
```

### 4. Create Product (Admin only)
```
POST /api/products
Body: {
  "name": "Bánh mì tươi",
  "description": "Bánh mì nướng tươi hàng ngày",
  "price": 150000,
  "categoryId": 1,
  "brandId": 1,
  "quantity": 50
}
```

---

## 📂 Project Structure

```
Oishipan/
├── OishipanAPI/              # API Backend
│   ├── Controllers/          # API endpoints
│   ├── Services/             # Business logic
│   ├── Models/               # Database models
│   ├── DTOs/                 # Data transfer objects
│   ├── Migrations/           # Database migrations
│   ├── Program.cs            # Startup config
│   └── appsettings.json      # Configuration
│
├── OishipanMVC/              # MVC Frontend
│   ├── Pages/                # Razor Pages
│   ├── Views/                # View templates
│   ├── Controllers/          # MVC controllers
│   ├── wwwroot/              # Static files
│   └── Program.cs            # Startup config
│
└── Documentation/
    ├── SWAGGER_GUIDE.md      # Swagger usage
    ├── QUICKSTART.md         # Quick start guide
    ├── CHANGES_SUMMARY.md    # Change log
    └── RUN_PROJECT.md        # This file
```

---

## ⚙️ Configuration Files

### appsettings.json (API)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=Oishipan;..."
  },
  "Jwt": {
    "SecretKey": "ReplaceWithAStrongKey...",
    "Issuer": "OishipanAPI",
    "Audience": "OishipanClients"
  },
  "Cloudinary": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
  }
}
```

### appsettings.json (MVC)
```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5000"
  },
  "Cloudinary": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
  }
}
```

---

## 🐛 Troubleshooting

### Problem: "Connection refused"
**Solution**: 
- Kiểm tra database server có chạy
- Verify connection string trong appsettings.json
- Restart Visual Studio

### Problem: "Port 5000 already in use"
**Solution**:
```bash
# Tìm process chiếm port 5000
netstat -ano | findstr :5000

# Kill process (Windows)
taskkill /PID {PID} /F
```

### Problem: "Migration failed"
**Solution**:
```bash
# Xóa database cũ và tạo mới
dotnet ef database drop
dotnet ef database update
```

### Problem: Swagger UI không hiển thị
**Solution**:
1. Đảm bảo API server đang chạy
2. Xóa cache browser (Ctrl+Shift+Del)
3. Thử incognito mode
4. Restart server

### Problem: "Unauthorized 401"
**Solution**:
- Token hết hạn → Đăng nhập lại
- Token không đúng format → Phải có `Bearer ` prefix
- Header sai → Phải là `Authorization` header

---

## 📊 API Port Configuration

### Development
- **API**: `http://localhost:5000`
- **MVC**: `http://localhost:5001`
- **Swagger**: `http://localhost:5000/swagger`

### Production
Configure trong `launchSettings.json`:
```json
"profiles": {
  "https": {
    "commandName": "Project",
    "dotnetRunMessages": true,
    "launchBrowser": false,
    "applicationUrl": "https://localhost:7001;http://localhost:5000",
    ...
  }
}
```

---

## 🔧 Environment Variables (Optional)

```bash
# Windows
set ASPNETCORE_ENVIRONMENT=Development

# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Development
```

---

## 📝 Useful Commands

### Build project
```bash
dotnet build
```

### Run tests
```bash
dotnet test
```

### Clean build
```bash
dotnet clean
dotnet build
```

### Check package versions
```bash
dotnet package search Microsoft.AspNetCore
```

### Update all packages
```bash
dotnet package update
```

---

## 🎯 Verify Setup Success

✅ Checklist:
- [ ] API server running (port 5000)
- [ ] Database connected
- [ ] MVC server running (port 5001)
- [ ] Swagger UI accessible
- [ ] Can login with admin credentials
- [ ] Can view products
- [ ] Can create order (after login)

---

## 📞 Contact & Support

- **API Docs**: `http://localhost:5000/swagger`
- **API Health**: `http://localhost:5000/health`
- **Documentation**: See `SWAGGER_GUIDE.md`

---

**Happy Coding! 🚀**

Last Updated: 2024
Version: 1.0.0
