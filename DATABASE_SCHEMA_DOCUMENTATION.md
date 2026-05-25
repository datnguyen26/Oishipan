# Database Schema Documentation - Oishipan

## Overview
Comprehensive database schema for the Oishipan e-commerce platform. This document describes all tables, columns, relationships, and constraints.

---

## Database Tables

### 1. **Accounts** (Tài khoản người dùng)
Stores user account information for customers and administrators.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| UserId | int (PK, Identity) | No | Primary key, auto-incremented |
| FullName | nvarchar(100) | No | User's full name |
| Email | nvarchar(100) | No | Email address (unique) |
| PhoneNumber | nvarchar(20) | No | Phone number (unique) |
| Password | nvarchar(255) | No | Hashed password (BCrypt, workFactor=12) |
| Role | nvarchar(50) | No | User role: "User" or "Admin" (Default: "User") |
| Address | nvarchar(200) | Yes | Delivery/billing address |
| Status | bit | No | Account active status (Default: true) |
| CreatedAt | datetime2 | No | Account creation timestamp |
| UpdatedAt | datetime2 | No | Last update timestamp |

**Constraints:**
- `Email` - UNIQUE
- `PhoneNumber` - UNIQUE

**Indexes:**
- `IX_Accounts_Email`
- `IX_Accounts_PhoneNumber`

---

### 2. **Categories** (Danh mục sản phẩm)
Product categories for organization and filtering.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| CategoryId | int (PK, Identity) | No | Primary key |
| CategoryName | nvarchar(100) | No | Category name (e.g., "Laptop", "Điện thoại") |
| Description | nvarchar(500) | Yes | Category description |
| Status | bit | No | Active status (Default: true) |
| CreatedAt | datetime2 | No | Creation timestamp |

**Indexes:**
- `IX_Categories_CategoryName`

---

### 3. **Brands** (Thương hiệu)
Product brands/manufacturers.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| BrandId | int (PK, Identity) | No | Primary key |
| BrandName | nvarchar(100) | No | Brand name (e.g., "Dell", "HP", "Asus") |
| Website | nvarchar(255) | Yes | Official website URL |
| Status | bit | No | Active status (Default: true) |
| CreatedAt | datetime2 | No | Creation timestamp |

**Indexes:**
- `IX_Brands_BrandName`

---

### 4. **Products** (Sản phẩm)
Product catalog with details, pricing, and inventory.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| ProductId | int (PK, Identity) | No | Primary key |
| ProductName | nvarchar(200) | No | Product name |
| Description | nvarchar(MAX) | Yes | Detailed product description |
| Price | decimal(10,2) | No | Unit price in VND |
| Stock | int | No | Available quantity (Default: 0) |
| CategoryId | int (FK) | No | Foreign key to Categories |
| BrandId | int (FK) | No | Foreign key to Brands |
| ImageUrl | nvarchar(500) | Yes | Product image URL (Cloudinary) |
| Specifications | nvarchar(MAX) | Yes | JSON specs (e.g., RAM, CPU, Storage) |
| Status | bit | No | Active status (Default: true) |
| CreatedAt | datetime2 | No | Creation timestamp |
| UpdatedAt | datetime2 | No | Last update timestamp |

**Foreign Keys:**
- `FK_Products_Categories` → Categories(CategoryId)
- `FK_Products_Brands` → Brands(BrandId)

**Indexes:**
- `IX_Products_CategoryId`
- `IX_Products_BrandId`
- `IX_Products_ProductName`

---

### 5. **Orders** (Đơn hàng)
Customer purchase orders.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| OrderId | int (PK, Identity) | No | Primary key |
| UserId | int (FK) | No | Foreign key to Accounts (customer) |
| OrderDate | datetime2 | No | Order creation date/time |
| TotalAmount | decimal(10,2) | No | Total order amount (VND) |
| Status | nvarchar(50) | No | Order status: "Pending", "Processing", "Shipped", "Delivered", "Cancelled" (Default: "Pending") |
| ShippingAddress | nvarchar(200) | No | Delivery address |
| PhoneNumber | nvarchar(20) | No | Contact phone for delivery |
| Notes | nvarchar(500) | Yes | Order notes/special instructions |
| CreatedAt | datetime2 | No | Creation timestamp |
| UpdatedAt | datetime2 | No | Last update timestamp |

**Foreign Keys:**
- `FK_Orders_Accounts` → Accounts(UserId)

**Indexes:**
- `IX_Orders_UserId`
- `IX_Orders_OrderDate`
- `IX_Orders_Status`

---

### 6. **OrderDetails** (Chi tiết đơn hàng)
Line items in each order (products ordered).

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| OrderDetailId | int (PK, Identity) | No | Primary key |
| OrderId | int (FK) | No | Foreign key to Orders |
| ProductId | int (FK) | No | Foreign key to Products |
| Quantity | int | No | Number of units ordered (≥ 1) |
| UnitPrice | decimal(10,2) | No | Price per unit at time of order |
| TotalPrice | decimal(10,2) | No | Quantity × UnitPrice |
| Note | nvarchar(500) | Yes | Line-item notes (e.g., color, size preferences) |
| CreatedAt | datetime2 | No | Creation timestamp |

**Foreign Keys:**
- `FK_OrderDetails_Orders` → Orders(OrderId) [Cascade Delete]
- `FK_OrderDetails_Products` → Products(ProductId)

**Indexes:**
- `IX_OrderDetails_OrderId`
- `IX_OrderDetails_ProductId`

---

### 7. **Payments** (Thanh toán)
Payment records for orders (VNPay or cash on delivery).

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| PaymentId | int (PK, Identity) | No | Primary key |
| OrderId | int (FK) | No | Foreign key to Orders |
| PaymentMethod | nvarchar(50) | No | Payment method: "VNPay", "COD" (Cash on Delivery) |
| Amount | decimal(10,2) | No | Payment amount (VND) |
| Status | nvarchar(50) | No | Payment status: "Pending", "Completed", "Failed", "Cancelled" (Default: "Pending") |
| TransactionId | nvarchar(100) | Yes | VNPay transaction ID (if applicable) |
| PaidAt | datetime2 | Yes | Payment completion timestamp |
| CreatedAt | datetime2 | No | Payment record creation timestamp |

**Foreign Keys:**
- `FK_Payments_Orders` → Orders(OrderId)

**Indexes:**
- `IX_Payments_OrderId`
- `IX_Payments_Status`

---

### 8. **Vouchers** (Mã giảm giá)
Promotional discount codes.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| VoucherId | int (PK, Identity) | No | Primary key |
| Code | nvarchar(50) | No | Unique voucher code (e.g., "SUMMER2026") |
| DiscountType | nvarchar(20) | No | Discount type: "Percentage" or "Fixed" |
| DiscountValue | decimal(10,2) | No | Discount amount (% or VND) |
| MaxUsage | int | No | Maximum times voucher can be used (-1 = unlimited) |
| CurrentUsage | int | No | Current usage count (Default: 0) |
| MinOrderAmount | decimal(10,2) | Yes | Minimum order amount to apply |
| StartDate | datetime2 | No | Voucher validity start date |
| EndDate | datetime2 | No | Voucher validity end date |
| Status | bit | No | Active status (Default: true) |
| CreatedAt | datetime2 | No | Creation timestamp |

**Constraints:**
- `Code` - UNIQUE
- `DiscountValue ≥ 0`
- `CurrentUsage ≤ MaxUsage` (if MaxUsage > 0)

**Indexes:**
- `IX_Vouchers_Code`
- `IX_Vouchers_Status`

---

## Entity Relationships (Diagram Text)

```
Accounts (1) ─── (Many) Orders
    │
    └─ Role: "User" or "Admin"
    
Orders (1) ─── (Many) OrderDetails
    │
    └─ (1) Payment (One payment per order)
    
Categories (1) ─── (Many) Products
    │
    └─ (1) Brand ─── (Many) Products
    
Products (1) ─── (Many) OrderDetails
```

---

## Authentication & Security

### Password Storage
- **Algorithm**: BCrypt (workFactor: 12)
- **Hash Length**: 60 characters
- **Example Hash**: `$2b$12$...` (Bcrypt format)

### JWT Token Structure
```json
{
  "nameid": "1",
  "email": "user@example.com",
  "role": "User",
  "UserId": "1",
  "iss": "OishipanAPI",
  "aud": "OishipanClients",
  "exp": 1685788800
}
```

**Token Expiry**: 60 minutes (configurable in appsettings.json)

---

## API Endpoints Summary

### Authentication Endpoints
- `POST /api/auth/register` - Register new account
- `POST /api/auth/login` - Login (returns JWT token)
- `GET /api/auth/profile/{userId}` - Get user profile
- `PUT /api/auth/update-profile/{userId}` - Update profile

### Product Endpoints
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/category/{categoryId}` - Filter by category
- `GET /api/products/brand/{brandId}` - Filter by brand

### Order Endpoints
- `POST /api/orders` - Create new order
- `GET /api/orders/{id}` - Get order details
- `GET /api/orders/user/{userId}` - Get user's orders
- `GET /api/orders` - Get all orders (admin)
- `PUT /api/orders/status` - Update order status (admin)
- `DELETE /api/orders/{id}` - Cancel order

### Voucher Endpoints
- `GET /api/vouchers/code/{code}` - Validate voucher
- `GET /api/vouchers` - Get all active vouchers
- `POST /api/vouchers` - Create voucher (admin)
- `DELETE /api/vouchers/{id}` - Delete voucher (admin)

---

## Sample Data

### Admin Account (Auto-created)
```
Email: admin@oishipan.com
Phone: 0123456789
Password: Admin@123 (hashed)
Role: Admin
```

### Seeding Script
Run `Database_Setup.sql` to initialize tables and sample data.

---

## Development Notes

1. **Connection String** (SQL Server):
   ```
   Server=DESKTOP-H3S2KUE\\HARYCUTE;Database=Oishipan;Trusted_Connection=true;
   ```

2. **EF Core Migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Swagger/OpenAPI Docs**:
   - Access at: `http://localhost:5000` (when running API)
   - Test endpoints directly from the UI
   - Use Bearer token for protected endpoints

---

**Last Updated**: May 2026  
**Version**: 1.0  
**Status**: Complete
