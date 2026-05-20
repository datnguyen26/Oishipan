# Quick Start Guide

## 🎯 Step-by-Step Setup (5 minutes)

### Step 1: Setup Database (2 minutes)

1. Open SQL Server Management Studio
2. Connect to: `DESKTOP-H3S2KUE\HARYCUTE`
3. Open file: `k:\Oishipan\Database_Setup.sql`
4. Click "Execute" or press F5
5. Verify database "Oishipan" is created

### Step 2: Configure API (2 minutes)

1. Open: `k:\Oishipan\OishipanAPI\appsettings.json`
2. Verify connection string:
   ```json
   "Server=DESKTOP-H3S2KUE\\HARYCUTE;Database=Oishipan;..."
   ```
3. Get Cloudinary account:
   - Go to cloudinary.com → Sign up (Free)
   - Copy your Cloud Name, API Key, API Secret
4. Update Cloudinary section:
   ```json
   "Cloudinary": {
       "CloudName": "your_cloud_name",
       "ApiKey": "your_api_key",
       "ApiSecret": "your_api_secret"
   }
   ```
5. Save file

### Step 3: Configure MVC (1 minute)

1. Open: `k:\Oishipan\OishipanMVC\appsettings.json`
2. Update Cloudinary (same as Step 2)
3. Save file

### Step 4: Run Projects

**Terminal 1 - API:**
```bash
cd k:\Oishipan\OishipanAPI
dotnet run
```
Wait for: "Now listening on: https://localhost:7001"

**Terminal 2 - MVC (new terminal):**
```bash
cd k:\Oishipan\OishipanMVC
dotnet run
```
Wait for: "Now listening on: https://localhost:XXXX"

### Step 5: Test Application

1. Open browser: `https://localhost:7002` (MVC)
2. Click "Register"
3. Create test account
4. Login
5. Browse products
6. Test order creation

## 🧪 Quick Testing

### Test API with Swagger
1. Go to: `https://localhost:7001/swagger`
2. Try endpoints directly

### Test with Postman
1. Get login token via: `POST /api/auth/login`
2. Copy token from response
3. Use token in Authorization header for other requests

## 📱 Key Login Credentials

Create your own via registration, or modify database directly:

**Test Admin Account** (if you manually insert):
- Email: admin@oishipan.com
- Password: Admin@123
- Role: Admin

## 🔗 Important URLs

| Component | URL |
|-----------|-----|
| API Documentation | https://localhost:7001/swagger |
| API Base | https://localhost:7001/api |
| MVC Application | https://localhost:7002 |
| Database | DESKTOP-H3S2KUE\HARYCUTE |

## ⚠️ Common Issues & Solutions

### "Database not found"
```
Solution: Run Database_Setup.sql again
```

### "Could not connect to database"
```
Solution: Verify SQL Server is running
         Check: DESKTOP-H3S2KUE\HARYCUTE is correct
         Run: sqlcmd -L to list servers
```

### "Cloudinary upload fails"
```
Solution: Verify API key/secret in appsettings.json
         Check CloudName is correct
         Test in Cloudinary dashboard first
```

### "API returns 401 Unauthorized"
```
Solution: Verify JWT token is included in header
         Check token is not expired
         Ensure Authorization: Bearer {token} format
```

### "MVC can't reach API"
```
Solution: Check API is running on :7001
         Verify BaseUrl in MVC appsettings.json
         Check HTTPS port is correct
```

## 📝 Default Port Numbers

- **API**: https://localhost:7001
- **MVC**: https://localhost:7002 (or auto-assigned)

If ports conflict, change in:
- API: `Properties/launchSettings.json`
- MVC: `Properties/launchSettings.json`

## 🎓 Project Architecture

```
Request Flow:
Browser → MVC Controller → API Client → API Controller → Service → Database
```

## 🔐 Security Features

- ✅ JWT Bearer Token Authentication
- ✅ Role-based Authorization
- ✅ BCrypt Password Hashing
- ✅ HTTPS/SSL
- ✅ CORS Configuration
- ✅ Secure Cloudinary Integration

## 📦 NuGet Packages Used

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.AspNetCore.Authentication.JwtBearer
- BCrypt.Net-Next
- CloudinaryDotNet
- Swashbuckle.AspNetCore

## 🎉 You're Ready!

All files are ready. Just:
1. Run Database_Setup.sql
2. Configure Cloudinary
3. Update connection strings
4. Run both projects
5. Start building!

## 📞 Support Resources

- Read: README.md (comprehensive guide)
- Read: SETUP_CHECKLIST.md (detailed checklist)
- Check: Code comments in each file
- Test: Swagger API documentation

---

**Happy coding! 🚀**
