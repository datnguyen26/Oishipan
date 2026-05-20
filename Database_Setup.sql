-- Create Database
CREATE DATABASE Oishipan;
GO

USE Oishipan;
GO

-- 1. Create Categories Table
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL
);

-- 2. Create Brands Table
CREATE TABLE Brands (
    BrandId INT PRIMARY KEY IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL,
    Website VARCHAR(100) NULL
);

-- 3. Create Products Table
CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Image NVARCHAR(MAX) NULL,
    Quantity INT NOT NULL,
    CategoryId INT NOT NULL,
    BrandId INT NOT NULL,
    Description NVARCHAR(MAX) NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId) ON DELETE CASCADE,
    FOREIGN KEY (BrandId) REFERENCES Brands(BrandId) ON DELETE CASCADE
);

-- 4. Create Accounts Table
CREATE TABLE Accounts (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber VARCHAR(15) NOT NULL UNIQUE,
    Password VARCHAR(MAX) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    Address NVARCHAR(MAX) NULL,
    Status BIT NOT NULL DEFAULT 1
);

-- 5. Create Orders Table
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Accounts(UserId) ON DELETE CASCADE
);

-- 6. Create OrderDetails Table
CREATE TABLE OrderDetails (
    OrderDetailId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Quantity INT NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE
);

-- 7. Create Payments Table (VNPay)
CREATE TABLE Payments (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    Vnp_TransactionNo VARCHAR(50) NOT NULL,
    Vnp_ResponseCode VARCHAR(10) NOT NULL,
    PaymentDate DATETIME NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE
);

-- 8. Create Vouchers Table
CREATE TABLE Vouchers (
    VoucherId INT PRIMARY KEY IDENTITY(1,1),
    Code VARCHAR(20) NOT NULL UNIQUE,
    DiscountValue DECIMAL(18,2) NOT NULL,
    ExpiryDate DATETIME NOT NULL
);

-- Create indexes for better query performance
CREATE INDEX idx_ProductsCategory ON Products(CategoryId);
CREATE INDEX idx_ProductsBrand ON Products(BrandId);
CREATE INDEX idx_OrdersUser ON Orders(UserId);
CREATE INDEX idx_OrderDetailsOrder ON OrderDetails(OrderId);
CREATE INDEX idx_OrderDetailsProduct ON OrderDetails(ProductId);
CREATE INDEX idx_PaymentsOrder ON Payments(OrderId);
CREATE INDEX idx_AccountsEmail ON Accounts(Email);
CREATE INDEX idx_VouchersCode ON Vouchers(Code);
