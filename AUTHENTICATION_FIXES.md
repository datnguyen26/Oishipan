# Authentication & Registration Fixes - Summary

## Issues Fixed

### 1. **Registration/Login Error: "This instance has already started one or more requests"**
**Root Cause:** The `ApiClient.cs` in MVC project had a critical bug:
- `HttpClient.BaseAddress` was being modified AFTER requests were sent
- `StringContent` was being created in retry lambdas but HttpClient property modification caused failures
- Once an HttpClient sends a request, its properties cannot be modified

**Solution:**
- Removed the problematic fallback base address retry logic
- Fixed all HTTP methods (`GetAsync`, `PostAsync`, `PutAsync`, `DeleteAsync`) to use correct SendAsync signature
- Simplified the code to just use the configured base address

**Files Modified:**
- `OishipanMVC/Services/ApiClient.cs` - Fixed all HTTP methods and removed fallback logic

### 2. **Missing JWT Authentication (Đăng nhập không trả token)**
**Root Cause:** 
- `appsettings.json` had no JWT configuration
- `Program.cs` was not configured with JWT authentication/authorization middleware
- `AuthService` was not generating or returning JWT tokens
- `LoginResponse` DTO had no `Token` field

**Solutions Applied:**

#### a) Added JWT Configuration
**File:** `OishipanAPI/appsettings.json`
```json
"Jwt": {
  "SecretKey": "ReplaceWithAStrongSecretKeyOfAtLeast32Chars!",
  "Issuer": "OishipanAPI",
  "Audience": "OishipanClients",
  "ExpiryMinutes": "60"
}
```

#### b) Configured JWT in Program.cs
**File:** `OishipanAPI/Program.cs`
- Added `Microsoft.AspNetCore.Authentication.JwtBearer` configuration
- Added `TokenValidationParameters` with issuer, audience, and signing key validation
- Registered `JwtTokenGenerator` as a singleton
- Enabled `app.UseAuthentication()` and `app.UseAuthorization()` middleware
- Added Swagger UI support with JWT Bearer authentication scheme

#### c) Updated AuthService to Generate Token
**File:** `OishipanAPI/Services/AuthService.cs`
- Injected `JwtTokenGenerator` into the constructor
- Modified `LoginAsync` to call `_jwtGenerator.GenerateToken()` on successful login
- Token is now returned in the `LoginResponse`

#### d) Added Token to Response DTO
**File:** `OishipanAPI/DTOs/AuthDto.cs`
- Added `public string Token { get; set; }` property to `LoginResponse` class

#### e) Added Swagger Documentation
**File:** `OishipanAPI/Program.cs`
- Configured Swagger with OpenAPI v1.0
- Added JWT Bearer security definition
- Swagger UI is available at application root (`/`)
- JWT support allows testing protected endpoints directly from Swagger

## Testing the Fixes

### 1. **Start API**
```bash
cd k:\Oishipan
dotnet run --project OishipanAPI
```
API will start at `http://localhost:5000`  
Swagger UI: `http://localhost:5000/swagger/ui`

### 2. **Test Registration**
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Nguyen Van A",
    "email": "test@example.com",
    "phoneNumber": "0123456789",
    "password": "Secret123",
    "address": "Hanoi"
  }'
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Đăng ký thành công. Vui lòng đăng nhập"
}
```

### 3. **Test Login**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Secret123"
  }'
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Đăng nhập thành công",
  "user": {
    "userId": 1,
    "fullName": "Nguyen Van A",
    "email": "test@example.com",
    "phoneNumber": "0123456789",
    "role": "User",
    "address": "Hanoi",
    "status": true
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 4. **Use Token for Protected Endpoints**
```bash
curl -X GET http://localhost:5000/api/auth/profile/1 \
  -H "Authorization: Bearer <TOKEN_FROM_LOGIN_RESPONSE>"
```

## Important - Security Configuration

### ⚠️ **CHANGE THE JWT SECRET KEY IN PRODUCTION**

Before deploying to production, update `Jwt:SecretKey` in `appsettings.json`:

```bash
# Generate a strong random key (on Windows PowerShell):
$bytes = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

Or use a tool to generate a random 32+ character string.

## Build & Run Instructions

### API
```bash
cd k:\Oishipan
dotnet run --project OishipanAPI
# API runs at http://localhost:5000
# Swagger at http://localhost:5000/ (redirects to swagger/ui)
```

### MVC (after API is running)
```bash
dotnet run --project OishipanMVC
# MVC runs at http://localhost:5002 (check launchSettings.json for exact port)
```

## What Changed From Original Code

| Component | Change |
|-----------|--------|
| `appsettings.json` | ✅ Added Jwt section |
| `Program.cs` | ✅ Added JWT auth config, middleware, Swagger |
| `AuthService.cs` | ✅ Added token generation on login |
| `AuthDto.cs` | ✅ Added Token field to LoginResponse |
| `ApiClient.cs` | ✅ Fixed HttpClient reuse bug, removed fallback logic |

## Verify It Works

1. **Build**: Both projects compile without errors ✅
2. **Register**: User can register with valid data ✅
3. **Login**: User receives JWT token on successful login ✅
4. **Protected Endpoints**: Token can be used in Authorization header ✅
5. **MVC Form**: Registration form in MVC no longer shows the "started requests" error ✅
