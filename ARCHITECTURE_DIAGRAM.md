# 🏗️ Oishipan Project - Architecture Overview (After Fixes)

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                         OISHIPAN E-COMMERCE                         │
└─────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────┐
│                      CLIENT APPLICATIONS                              │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────────┐              ┌──────────────────┐              │
│  │  MVC Application │              │  Swagger UI      │              │
│  │  Port: 3000      │              │  Port: 5000      │              │
│  ├──────────────────┤              ├──────────────────┤              │
│  │ • Login/Register │              │ • API Testing    │              │
│  │ • Browse Products│              │ • Endpoint Docs  │              │
│  │ • Admin Panel    │              │ • Try It Out     │              │
│  │ • User Profile   │              │ • All Endpoints  │              │
│  └────────┬─────────┘              └────────┬─────────┘              │
│           │                                 │                        │
└───────────┼─────────────────────────────────┼────────────────────────┘
            │                                 │
            └────────────────────┬────────────┘
                                 │
                    ┌────────────▼─────────────┐
                    │  HTTP Requests/Responses │
                    │  JSON Data Exchange      │
                    │  Authentication (JWT)    │
                    └────────────┬─────────────┘
                                 │
┌────────────────────────────────▼──────────────────────────────────────┐
│                       API APPLICATION                                  │
│                      Port: 5000 (ASP.NET Core)                        │
├────────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  ┌─────────────────────────────────────────────────────────────┐     │
│  │                    API CONTROLLERS                          │     │
│  ├─────────────────────────────────────────────────────────────┤     │
│  │                                                             │     │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐ ┌──────────┐   │     │
│  │  │   Auth   │  │ Products │  │  Upload  │ │  Orders  │   │     │
│  │  │Controller│  │Controller│  │Controller│ │Controller│   │     │
│  │  └────┬─────┘  └────┬─────┘  └────┬─────┘ └────┬─────┘   │     │
│  │       │ /api/auth   │ /api/     │ /api/   │ /api/orders  │     │
│  │       │ • Login     │ products  │ upload  │            │     │
│  │       │ • Register  │ • CRUD    │ • File  │ • Create   │     │
│  │       │ • Profile   │ • Auth    │ • Valid │ • Manage   │     │
│  │       │             │ • Search  │ • Cloud │            │     │
│  │       │             │           │ inary   │            │     │
│  │  └────┬─────┘  └────┬─────┘  └────┬─────┘ └────┬─────┘   │     │
│  │                                                             │     │
│  └──────────────────────┬────────────────────────────────────┘     │
│                         │                                           │
│  ┌──────────────────────▼──────────────────────────────────┐       │
│  │              BUSINESS LOGIC LAYER                       │       │
│  ├──────────────────────────────────────────────────────────┤       │
│  │                                                          │       │
│  │  ┌──────────────┐  ┌──────────────┐                    │       │
│  │  │ AuthService  │  │ProductService│                    │       │
│  │  └──────┬───────┘  └──────┬───────┘                    │       │
│  │         │                 │                             │       │
│  │  ┌──────▼────────┐  ┌─────▼───────┐  ┌──────────────┐ │       │
│  │  │ CloudinaryService  │ OrderService│  │VoucherService│ │       │
│  │  └──────┬────────┘  └─────┬───────┘  └──────────────┘ │       │
│  │         │                 │                             │       │
│  └─────────┼─────────────────┼─────────────────────────────┘       │
│            │                 │                                      │
│  ┌─────────▼──────────────────▼─────────────────────────────┐      │
│  │            DATA ACCESS LAYER (Entity Framework)          │      │
│  ├──────────────────────────────────────────────────────────┤      │
│  │  • User/Account  • Product  • Order  • OrderDetail       │      │
│  │  • Category      • Brand    • Cart   • Voucher           │      │
│  └──────────────────┬──────────────────────────────────────┘      │
│                     │                                              │
└─────────────────────┼──────────────────────────────────────────────┘
                      │
┌─────────────────────▼──────────────────────────────────────────────┐
│                    EXTERNAL SERVICES                               │
├──────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌────────────────────────┐      ┌────────────────────────┐       │
│  │   SQL SERVER DATABASE  │      │    CLOUDINARY          │       │
│  │                        │      │    (Image Storage)     │       │
│  │ • Tables               │      │                        │       │
│  │ • Relationships        │      │ • Upload Images        │       │
│  │ • Constraints          │      │ • Store URLs           │       │
│  │ • Indexes              │      │ • Optimize Images      │       │
│  └────────────────────────┘      └────────────────────────┘       │
│                                                                     │
└──────────────────────────────────────────────────────────────────────┘
```

---

## Data Flow for Key Operations

### 1. User Login Flow
```
User Browser                   MVC App                    API Server
     │                           │                             │
     ├──POST /dang-nhap──────→  │                             │
     │                           ├──POST /api/auth/login──→   │
     │                           │                             ├──DB Query
     │                           │   JWT Token + User Data  │
     │                  ←────────┤──────────────────────────┤
     │                           │                             │
     │ Cookie Set                │                             │
     │←──────────────────────────┤                             │
     │ Redirect to Home          │                             │
     │                           │                             │
```

### 2. Product Creation with Image Upload
```
Admin User                  MVC Controller              API Server              Cloudinary
     │                            │                           │                    │
     ├─Form + Image───────────→  │                           │                    │
     │                            ├──POST /api/upload────→   │                    │
     │                            │                           ├──Upload───────→   │
     │                            │                           │                    │
     │                            │                    ←──────┤──Image URL     ┌──┤
     │                            │              ←────────────┤─────────────→  │URL│
     │                            ├──POST /api/products───→   │                └───┘
     │                            │ (with Cloudinary URL)     ├──Save to DB
     │                            │            ←──────────────┤
     │                            │ Success Response          │
     │                  ←─────────┤──────────────────────────→│
     │ Product Created            │                           │
     │                            │                           │
```

### 3. Authorization & Access Control
```
User Request              MVC/API                    Authentication
     │                        │                             │
     ├──GET /admin──────────→ │                             │
     │                        ├──Check [Authorize]─────→   │
     │                        │                             ├──No Cookie/JWT
     │                        │                       ←─────┤─Not Authorized
     │                        │                             │
     │ ←──Redirect to Login───┤                             │
     │                        │                             │
     │──POST /dang-nhap───→   │                             │
     │(email/password)        ├──Validate────────────────→ │
     │                        │                             ├──JWT/Cookie
     │                        │ ←──Generate Token──────────┤
     │ ←──Set Cookie──────────┤                             │
     │                        │                             │
     ├──GET /admin───────→    │                             │
     │                        ├──Check [Authorize]─────→   │
     │                        │                             ├──Has Cookie
     │                        │                       ←─────┤─Authorized
     │ ←──Admin Dashboard─────┤                             │
     │                        │                             │
```

---

## API Endpoints Map

### Authentication Routes
```
┌─ /api/auth/
│  ├─ POST   /login              (Public) → Login with email/password
│  ├─ POST   /register           (Public) → Register new account
│  ├─ GET    /profile/{id}       (Auth)   → Get user profile
│  └─ PUT    /update-profile/{id}(Auth)   → Update profile
```

### Products Routes
```
┌─ /api/products/
│  ├─ GET    /                   (Public) → List all products
│  ├─ GET    /{id}               (Public) → Get product details
│  ├─ GET    /category/{catId}   (Public) → Products by category
│  ├─ GET    /brand/{brandId}    (Public) → Products by brand
│  ├─ POST   /                   (Admin)  → Create product
│  ├─ PUT    /{id}               (Admin)  → Update product
│  ├─ DELETE /{id}               (Admin)  → Delete product
│  └─ POST   /{id}/upload-image  (Admin)  → Upload product image
```

### Upload Routes ✅ NEW
```
┌─ /api/upload/
│  └─ POST   /                   (Public) → Upload image to Cloudinary
```

### Documentation
```
┌─ /swagger                      (Public) → API Documentation
├─ /health                       (Public) → Health check
└─ /                             (Public) → API info
```

---

## Security & Authorization Model

### Role-Based Access Control
```
┌─────────────────────────────────────────────────────────┐
│                    USER ROLES                           │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐              │
│  │  ADMIN   │  │  STAFF   │  │   USER   │              │
│  ├──────────┤  ├──────────┤  ├──────────┤              │
│  │ • Admin  │  │ • Admin  │  │ • View   │              │
│  │   Panel  │  │   Panel  │  │   Prods  │              │
│  │ • Users  │  │ • Prod   │  │ • Browse │              │
│  │ • Prods  │  │   CRUD   │  │ • Orders │              │
│  │ • Orders │  │ • Orders │  │ • Profile│              │
│  │ • All    │  │ • Limited│  │ • Browse │              │
│  └──────────┘  └──────────┘  └──────────┘              │
│                                                         │
└─────────────────────────────────────────────────────────┘

Authorization Attributes:
├─ [Authorize]                    → Any authenticated user
├─ [Authorize(Roles = "Admin")]   → Only Admin users
└─ [Authorize(Roles = "Admin,Staff")] → Admin or Staff
```

---

## Technology Stack

```
┌────────────────────────────────────────────────┐
│                 FRONTEND                       │
├────────────────────────────────────────────────┤
│ • ASP.NET Core MVC (C#)                       │
│ • Razor Templates                             │
│ • HTML5 / CSS3 / JavaScript                   │
│ • Bootstrap (assumed)                         │
└────────────────────────────────────────────────┘
                    │
                    ↓
┌────────────────────────────────────────────────┐
│                 API LAYER                      │
├────────────────────────────────────────────────┤
│ • ASP.NET Core Web API                        │
│ • Entity Framework Core (ORM)                 │
│ • JWT Authentication                         │
│ • Swagger/OpenAPI Documentation               │
└────────────────────────────────────────────────┘
                    │
                    ↓
┌────────────────────────────────────────────────┐
│              DATA STORAGE                      │
├────────────────────────────────────────────────┤
│ • SQL Server (Database)                       │
│ • Cloudinary (Image Storage)                  │
└────────────────────────────────────────────────┘
```

---

## Key Improvements Made

### ✅ Before Fixes
```
❌ Swagger not accessible
❌ Login returns NotFound error
❌ Image upload endpoint missing
❌ No file validation for uploads
❌ Admin area not secured
❌ Authorization policies missing
❌ Duplicate upload code
```

### ✅ After Fixes
```
✅ Swagger always accessible
✅ Login/Register working correctly
✅ Dedicated upload endpoint with validation
✅ File type and size validation
✅ Admin area properly secured
✅ Role-based authorization enforced
✅ Clean code with proper separation
```

---

## Deployment Architecture

```
Development Environment
├─ API: localhost:5000
├─ MVC: localhost:3000
├─ DB: SQL Server (local)
└─ Images: Cloudinary

Production Environment
├─ API: api.oishipan.com (Docker/IIS)
├─ MVC: www.oishipan.com (Docker/IIS)
├─ DB: SQL Server (Azure)
└─ Images: Cloudinary CDN
```

---

## Performance Considerations

```
┌────────────────────────────────────────────┐
│          OPTIMIZATION FEATURES              │
├────────────────────────────────────────────┤
│ ✓ Async/await for all I/O operations      │
│ ✓ Connection pooling (DB)                 │
│ ✓ Cloudinary automatic image optimization │
│ ✓ Image compression and transformations   │
│ ✓ Cache headers for static files          │
│ ✓ CORS enabled for API                    │
│ ✓ Error handling and logging              │
└────────────────────────────────────────────┘
```

---

## Conclusion

The Oishipan project now has:
- ✅ Fully functional authentication system
- ✅ Complete Cloudinary image integration
- ✅ Proper role-based authorization
- ✅ Production-ready API with Swagger
- ✅ Clean, maintainable architecture
- ✅ Comprehensive documentation

**Status: 🟢 READY FOR DEPLOYMENT**
