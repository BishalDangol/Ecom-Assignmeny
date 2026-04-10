/*
    GADGET HUB - ELECTRONICS STORE DATABASE INITIALIZATION SCRIPT
    This script creates the necessary tables and populates them with categories and electronic products.
*/

-- 1. Create Core Tables
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'categories')
BEGIN
    CREATE TABLE dbo.categories(
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [name] VARCHAR(255) NOT NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'products')
BEGIN
    CREATE TABLE dbo.products(
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        category_id INT NOT NULL REFERENCES dbo.categories(id),
        [name] VARCHAR(255) NOT NULL,
        [description] VARCHAR(MAX) NOT NULL,
        stock INT NOT NULL DEFAULT (0),
        price DECIMAL(10,2) NOT NULL,
        [image] VARCHAR(255) NULL,
        is_show BIT NOT NULL DEFAULT (1),
        created_at DATETIME2(0) NOT NULL DEFAULT (GETDATE()),
        updated_at DATETIME2(0) NOT NULL DEFAULT (GETDATE())
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'admins')
BEGIN
    CREATE TABLE dbo.admins(
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        full_name VARCHAR(255) NULL,
        username VARCHAR(255) NOT NULL UNIQUE,
        [password] VARCHAR(255) NOT NULL,
        role VARCHAR(50) NULL,
        created_at DATETIME2(0) NOT NULL DEFAULT (GETDATE()),
        updated_at DATETIME2(0) NOT NULL DEFAULT (GETDATE())
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'members')
BEGIN
    CREATE TABLE dbo.members(
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        full_name VARCHAR(255) NULL,
        username VARCHAR(255) NOT NULL UNIQUE,
        email VARCHAR(255) NOT NULL UNIQUE,
        [password] VARCHAR(255) NOT NULL,
        phone VARCHAR(50) NULL,
        created_at DATETIME2(0) NOT NULL DEFAULT (GETDATE()),
        updated_at DATETIME2(0) NOT NULL DEFAULT (GETDATE())
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'orders')
BEGIN
    CREATE TABLE dbo.orders(
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        order_code VARCHAR(50) NOT NULL UNIQUE,
        member_id INT NOT NULL REFERENCES dbo.members(id),
        ship_name VARCHAR(255) NULL,
        ship_phone VARCHAR(50) NULL,
        [status] VARCHAR(50) NULL,
        total_qty INT NOT NULL,
        total_amount DECIMAL(10,2) NOT NULL,
        payment VARCHAR(255) NOT NULL,
        order_date DATETIME2(0) NOT NULL DEFAULT (GETDATE()),
        created_at DATETIME2(0) NOT NULL DEFAULT (GETDATE()),
        updated_at DATETIME2(0) NOT NULL DEFAULT (GETDATE())
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'order_items')
BEGIN
    CREATE TABLE dbo.order_items(
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        order_id INT NOT NULL REFERENCES dbo.orders(id),
        product_id INT NOT NULL REFERENCES dbo.products(id),
        quantity INT NOT NULL,
        amount DECIMAL(10,2) NOT NULL,
        created_at DATETIME2(0) NOT NULL DEFAULT (GETDATE())
    );
END;

-- 2. Seed Data
-- Clear existing data if needed (Optional: uncomment below)
-- DELETE FROM dbo.order_items; DELETE FROM dbo.orders; DELETE FROM dbo.products; DELETE FROM dbo.categories;

-- Categories
INSERT INTO dbo.categories ([name]) VALUES ('Smartphones');
INSERT INTO dbo.categories ([name]) VALUES ('Laptops');
INSERT INTO dbo.categories ([name]) VALUES ('Audio & Headphones');
INSERT INTO dbo.categories ([name]) VALUES ('Cameras');
INSERT INTO dbo.categories ([name]) VALUES ('Accessories');

-- Products
DECLARE @Smartphones INT = (SELECT id FROM categories WHERE [name] = 'Smartphones');
DECLARE @Laptops INT = (SELECT id FROM categories WHERE [name] = 'Laptops');
DECLARE @Audio INT = (SELECT id FROM categories WHERE [name] = 'Audio & Headphones');
DECLARE @Cameras INT = (SELECT id FROM categories WHERE [name] = 'Cameras');

INSERT INTO dbo.products (category_id, [name], [description], stock, price, is_show) VALUES 
(@Smartphones, 'iPhone 15 Pro', 'Experience the power of titanium and the A17 Pro chip.', 15, 999.00, 1),
(@Smartphones, 'Samsung Galaxy S24 Ultra', 'AI-powered smartphone with 200MP camera and S-Pen.', 18, 1299.00, 1),
(@Laptops, 'MacBook Air M2', 'Thin, light, and incredibly fast with the M2 chip.', 10, 1199.00, 1),
(@Laptops, 'Dell XPS 13', 'Premium design and stunning display for ultimate productivity.', 12, 1099.00, 1),
(@Audio, 'Sony WH-1000XM5', 'Industry-leading noise cancelling headphones.', 25, 349.00, 1),
(@Audio, 'AirPods Pro (2nd Gen)', 'Immersive sound with active noise cancellation.', 30, 249.00, 1),
(@Cameras, 'Fujifilm X-T5', 'Beautifully designed mirrorless camera for enthusiasts.', 5, 1699.00, 1),
(@Cameras, 'DJI Mini 4 Pro', 'Pro-level drone in a compact, portable package.', 8, 759.00, 1);

-- Default Admin (Password: 123456 hashed with SHA256)
-- admin / 123456
INSERT INTO dbo.admins (full_name, username, [password], role)
VALUES ('Admin User', 'admin', '8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92', 'superadmin');

PRINT 'Database setup and seeding completed successfully.';
