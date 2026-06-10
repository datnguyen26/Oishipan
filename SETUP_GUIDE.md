# 🚀 Setup Hướng Dẫn - Oishipan Voucher Admin System

Hướng dẫn cài đặt hoàn chỉnh cho cả Backend (API) và Frontend (React App).

## 📋 Điều Kiện Tiên Quyết

- **Node.js** 18+ (download từ nodejs.org)
- **.NET SDK** 8.0+ (download từ dotnet.microsoft.com)
- **SQL Server** 2019+ hoặc SQL Server Express
- **Git** (optional)
- **IDE**: Visual Studio Code hoặc Visual Studio 2022

## 🔧 Backend Setup (OishipanAPI)

### Bước 1: Chuẩn Bị Database

```bash
# Mở SQL Server Management Studio
# Tạo database "OishipanDB" (hoặc tên khác)
# Kiểm tra connection string trong appsettings.json
```

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\HARYCUTE;Database=OishipanDB;Trusted_Connection=true;"
  }
}
```

### Bước 2: Restore Dependencies & Build

```bash
cd k:\Oishipan\OishipanAPI

# Restore packages
dotnet restore

# Build project
dotnet build
```

### Bước 3: Chạy Database Migration

```bash
# Áp dụng migrations
dotnet ef database update

# Nếu lỗi, có thể cần:
# dotnet ef database drop --force
# dotnet ef database update
```

### Bước 4: Chạy API Server

```bash
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to stop
```

### Bước 5: Kiểm Tra API

```bash
# Mở browser
http://localhost:5000/swagger

# Hoặc test với curl
curl http://localhost:5000/api/vouchers
# Sẽ trả lỗi 401 (Unauthorized) - bình thường, cần token
```

---

## 🎨 Frontend Setup (React App)

### Bước 1: Install Node Dependencies

```bash
cd k:\Oishipan\OishipanMVC\wwwroot\admin-voucher

# Install packages
npm install
# Hoặc dùng yarn
yarn install
```

**Chờ hoàn tất (~2-3 phút tùy tốc độ internet)**

### Bước 2: Cấu Hình Environment

```bash
# Copy file
cp .env.example .env.local

# Edit .env.local
VITE_APP_API_URL=http://localhost:5000/api
```

### Bước 3: Chạy Development Server

```bash
npm run dev
# Hoặc
yarn dev
```

**Expected Output:**
```
  VITE v5.0.8  ready in xxx ms

  ➜  Local:   http://localhost:5173/
  ➜  press h to show help
```

### Bước 4: Mở Ứng Dụng

```
Mở browser: http://localhost:5173
```

---

## 🔑 Đăng Nhập & Test

### Admin Credentials

```
Email: admin@oishipan.com
Password: Admin@123
```

### Test Flow

1. ✅ Mở http://localhost:5173 (React app)
2. ✅ Đăng nhập bằng admin account
3. ✅ Chuyển sang trang Quản Lý Voucher
4. ✅ Thực hiện các tác vụ:
   - Tạo voucher mới
   - Chỉnh sửa voucher
   - Xóa voucher
   - Tìm kiếm & lọc
   - Sao chép mã

---

## 🛠️ Troubleshooting

### ❌ "Cannot connect to database"

**Giải pháp:**
```bash
# Kiểm tra connection string
# Mở SQL Server Management Studio
# Kiểm tra server name và database tồn tại
# Nếu cần, chạy lại migration

dotnet ef database drop --force
dotnet ef database update
```

### ❌ "Port 5000 đang được sử dụng"

**Giải pháp:**
```bash
# Đổi port trong Properties/launchSettings.json
# Hoặc kill process
# Windows:
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### ❌ "CORS Error"

**Giải pháp:**
```csharp
// OishipanAPI/Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", builder =>
    {
        builder.WithOrigins("http://localhost:5173", "http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

app.UseCors("AllowReact");
```

### ❌ "Cannot GET /api/vouchers"

**Giải pháp:**
1. Kiểm tra OishipanAPI đang chạy
2. Kiểm tra token được gửi đúng
3. Kiểm tra trong DevTools → Network tab

### ❌ "401 Unauthorized"

**Giải pháp:**
1. Đăng nhập lại
2. Kiểm tra token có hết hạn không
3. Kiểm tra role là "Admin"

### ❌ "npm install fails"

**Giải pháp:**
```bash
# Clear cache
npm cache clean --force

# Xóa node_modules
rm -r node_modules
rm package-lock.json

# Cài lại
npm install
```

---

## 📊 Development Workflow

### Terminal Tab 1 - Backend API

```bash
cd k:\Oishipan\OishipanAPI
dotnet run
# Đang chạy tại http://localhost:5000
```

### Terminal Tab 2 - Frontend App

```bash
cd k:\Oishipan\OishipanMVC\wwwroot\admin-voucher
npm run dev
# Đang chạy tại http://localhost:5173
```

### Terminal Tab 3 - SQL Server Management

```bash
# Tuỳ chọn: Mở SSMS để monitor database
```

---

## 🧪 Testing

### Unit Tests API

```bash
cd k:\Oishipan\OishipanAPI

# Chạy tests (nếu có)
dotnet test
```

### Component Tests (React)

```bash
cd k:\Oishipan\OishipanMVC\wwwroot\admin-voucher

# Nếu thêm testing library
npm test
```

### E2E Testing

```bash
# Sử dụng Playwright hoặc Cypress
npm install -D @playwright/test

# Chạy E2E tests
npm run test:e2e
```

---

## 📦 Build & Deploy

### Build Frontend

```bash
cd k:\Oishipan\OishipanMVC\wwwroot\admin-voucher

# Production build
npm run build

# Output: dist/ folder
# Deploy dist/ contents to web server
```

### Build Backend

```bash
cd k:\Oishipan\OishipanAPI

# Publish for production
dotnet publish -c Release -o ./publish

# Deploy ./publish folder
```

### Docker Deployment (Optional)

**Backend Dockerfile:**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 5000
ENTRYPOINT ["dotnet", "OishipanAPI.dll"]
```

**Frontend Dockerfile:**
```dockerfile
FROM node:18-alpine AS builder
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:18-alpine
WORKDIR /app
RUN npm install -g serve
COPY --from=builder /app/dist ./dist
EXPOSE 3000
CMD ["serve", "-s", "dist"]
```

---

## 🔍 Debug Mode

### Debug API

```bash
# Visual Studio Code: Install C# extension
# F5 to start debugging
# Set breakpoints
```

### Debug React App

```bash
# React DevTools Browser Extension
# Open DevTools (F12)
# Components tab to inspect components
# Profiler tab to analyze performance
```

---

## 📝 Các File Quan Trọng

| File | Mục Đích |
|------|---------|
| `OishipanAPI/Models/Voucher.cs` | Database model |
| `OishipanAPI/Controllers/VouchersController.cs` | API endpoints |
| `OishipanAPI/Services/VoucherService.cs` | Business logic |
| `OishipanMVC/wwwroot/admin-voucher/src/App.jsx` | Main React app |
| `OishipanMVC/wwwroot/admin-voucher/src/services/voucherService.js` | API client |
| `.env.local` | Environment config |

---

## ✅ Checklist Hoàn Thành

- ✅ Node.js 18+ installed
- ✅ .NET SDK 8.0+ installed
- ✅ SQL Server running
- ✅ Database created
- ✅ Backend dependencies restored
- ✅ Database migrations applied
- ✅ Backend API running (port 5000)
- ✅ Frontend dependencies installed
- ✅ Environment configured (.env.local)
- ✅ Frontend dev server running (port 5173)
- ✅ Logged in as admin
- ✅ Created test voucher
- ✅ All features tested

---

## 📞 Support & Resources

- **Backend Docs**: [BACKEND_API_CHANGES.md](./BACKEND_API_CHANGES.md)
- **Frontend Docs**: [admin-voucher/README.md](./OishipanMVC/wwwroot/admin-voucher/README.md)
- **Admin Credentials**: [ADMIN_CREDENTIALS.md](./ADMIN_CREDENTIALS.md)
- **API Swagger**: http://localhost:5000/swagger

---

**Chúc bạn thành công! 🎉**

Nếu gặp vấn đề, hãy kiểm tra:
1. Tất cả services đang chạy
2. Ports không bị xung đột
3. Database connection string
4. Admin credentials đúng
5. Token JWT không hết hạn

---

**Cập Nhật Lần Cuối**: 03/06/2026
