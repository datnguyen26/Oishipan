# Oishipan - MVC & API Project

A full-stack e-commerce application for selling cakes with MVC frontend and ASP.NET Core API backend.

## Project Structure

```
OishipanAPI/           # ASP.NET Core Web API
├── Controllers/       # API Controllers
├── Models/           # Entity Framework Models & DbContext
├── Services/         # Business Logic Services
├── DTOs/             # Data Transfer Objects
├── Utilities/        # Helper Classes (JWT, Password)
├── Middlewares/      # Custom Middlewares
├── appsettings.json  # Configuration
└── Program.cs        # Application Startup

OishipanMVC/          # ASP.NET Core MVC
├── Controllers/      # MVC Controllers
├── Views/           # Razor Views
├── Services/        # API Client Services
├── Models/          # View Models
├── wwwroot/         # Static Files (CSS, JS, Images)
├── appsettings.json # Configuration
└── Program.cs       # Application Startup
```

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code with C# extension
- Cloudinary Account (for image uploads)

## Database Setup

### 1. Create Database

Execute the [Database_Setup.sql](Database_Setup.sql) script on your SQL Server:

```bash
sqlcmd -S "DESKTOP-H3S2KUE\HARYCUTE" -d master -i Database_Setup.sql
```

Or use SQL Server Management Studio:
- Open Database_Setup.sql
- Execute the script

### 2. Database Connection String

Update `OishipanAPI/appsettings.json`:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-H3S2KUE\\HARYCUTE;Database=Oishipan;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

## API Configuration

### 1. JWT Settings

Update `OishipanAPI/appsettings.json`:

```json
"Jwt": {
    "SecretKey": "your_super_secret_key_min_32_characters_long!",
    "Issuer": "OishipanAPI",
    "Audience": "OishipanClient",
    "ExpiryMinutes": 60
}
```

### 2. Cloudinary Configuration

1. Sign up at [Cloudinary.com](https://cloudinary.com)
2. Get your credentials from Dashboard
3. Update `OishipanAPI/appsettings.json`:

```json
"Cloudinary": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
}
```

### 3. Run API Project

```bash
cd OishipanAPI
dotnet restore
dotnet run
```

API will run on: `https://localhost:7001`

## MVC Configuration

### 1. Update API URL

In `OishipanMVC/appsettings.json`:

```json
"ApiSettings": {
    "BaseUrl": "https://localhost:7001"
}
```

### 2. Run MVC Project

```bash
cd OishipanMVC
dotnet restore
dotnet run
```

MVC will run on: `https://localhost:7002` (or default HTTPS port)

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/profile` - Get user profile (Requires Auth)
- `PUT /api/auth/update-profile` - Update profile (Requires Auth)

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product details
- `GET /api/products/category/{categoryId}` - Get products by category
- `GET /api/products/brand/{brandId}` - Get products by brand
- `POST /api/products` - Create product (Admin/Staff only)
- `PUT /api/products/{id}` - Update product (Admin/Staff only)
- `DELETE /api/products/{id}` - Delete product (Admin/Staff only)
- `POST /api/products/{id}/upload-image` - Upload product image (Admin/Staff only)

### Orders
- `POST /api/orders` - Create order (Requires Auth)
- `GET /api/orders/{id}` - Get order details (Requires Auth)
- `GET /api/orders/user/my-orders` - Get user's orders (Requires Auth)
- `GET /api/orders` - Get all orders (Admin/Staff only)
- `PUT /api/orders/{id}/status` - Update order status (Admin/Staff only)
- `DELETE /api/orders/{id}/cancel` - Cancel order (Requires Auth)

### Vouchers
- `GET /api/vouchers/{code}` - Get voucher details
- `GET /api/vouchers/{code}/validate` - Validate voucher
- `GET /api/vouchers` - Get all vouchers (Admin only)
- `POST /api/vouchers` - Create voucher (Admin only)
- `DELETE /api/vouchers/{id}` - Delete voucher (Admin only)

## Database Schema

### Tables

1. **Categories** - Product categories
2. **Brands** - Product brands
3. **Products** - Product inventory
4. **Accounts** - User accounts
5. **Orders** - Customer orders
6. **OrderDetails** - Order line items
7. **Payments** - VNPay payment records
8. **Vouchers** - Discount vouchers

## Authentication Flow

1. User registers via `/api/auth/register`
2. User logs in via `/api/auth/login`
3. API returns JWT token
4. MVC stores token in session
5. MVC includes token in Authorization header for API calls
6. API validates token for protected endpoints

## File Upload

1. Upload product image via `/api/products/{id}/upload-image`
2. Cloudinary stores and returns secure URL
3. Product record updated with image URL

## Default User Roles

- **User** - Regular customer
- **Staff** - Can manage products
- **Admin** - Full access

## Security Features

- ✅ JWT Token Authentication
- ✅ BCrypt Password Hashing
- ✅ SQL Server Integration
- ✅ Cloudinary Image Storage
- ✅ CORS Configuration
- ✅ Authorization by Role
- ✅ HTTPS Support

## Frontend Features

- User Registration & Login
- Product Browsing & Search
- Order Management
- Responsive Design (Bootstrap 5)

## Backend Features

- RESTful API with Swagger
- Entity Framework Core ORM
- Dependency Injection
- Service Layer Architecture
- Async/Await Pattern
- Transaction Support

## Development Tips

### Enable CORS for Testing

In `OishipanAPI/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
```

### Test API with Swagger

1. Run API project
2. Navigate to `https://localhost:7001/swagger`
3. Use Swagger UI to test endpoints

### Database Migrations (if needed)

```bash
dotnet ef migrations add InitialCreate --project OishipanAPI
dotnet ef database update --project OishipanAPI
```

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check server name: `DESKTOP-H3S2KUE\HARYCUTE`
- Ensure database `Oishipan` exists
- Verify credentials and permissions

### JWT Token Issues
- Ensure SecretKey is at least 32 characters
- Token expires after configured minutes
- Include "Bearer " prefix in Authorization header

### Cloudinary Upload Issues
- Verify credentials in appsettings.json
- Check folder permissions
- Ensure file size is reasonable

### CORS Issues
- Check API CORS policy matches MVC origin
- Verify API is running on correct port
- Clear browser cache

## Next Steps

1. Run database setup script
2. Configure Cloudinary account
3. Update appsettings.json files
4. Run API project: `dotnet run` in OishipanAPI
5. Run MVC project: `dotnet run` in OishipanMVC
6. Navigate to MVC application
7. Test registration, login, and product browsing

## Support

For issues or questions, check the code comments and swagger documentation at `/swagger` endpoint.

## License

Private Project - All Rights Reserved
