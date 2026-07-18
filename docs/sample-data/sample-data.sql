USE ConstructFlowDb;
GO

-- =============================================
-- SAMPLE DATA FOR CONSTRUCTFLOW
-- Run this after your schema is deployed
-- This creates a realistic demo dataset
-- =============================================

-- =============================================
-- 1. AUTH — Create demo users
-- Password for all users: Demo@1234
-- BCrypt hash generated for Demo@1234 with workFactor 12
-- =============================================

INSERT INTO AUTH.Users (FullName, Email, PasswordHash, Role, IsActive, CreatedAt)
VALUES 
('Amal Das', 'amal@constructflow.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeKr1HfLfD0g8C5Aq', 'Admin', 1, GETUTCDATE()),
('Priya Nair', 'priya@constructflow.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeKr1HfLfD0g8C5Aq', 'Manager', 1, GETUTCDATE()),
('Rahul Menon', 'rahul@constructflow.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeKr1HfLfD0g8C5Aq', 'User', 1, GETUTCDATE());
GO

-- =============================================
-- 2. PROJECTS
-- =============================================

INSERT INTO PRJ.Projects (Name, Code, Location, Status, StartDate, EndDate, Budget, CreatedAt)
VALUES
('Riverside Towers', 'PRJ-001', 'Kochi, Kerala', 2, '2025-01-15', NULL, 45000000.00, '2025-01-15'),
('NH66 Highway Overpass', 'PRJ-002', 'Kozhikode, Kerala', 2, '2025-03-01', NULL, 120000000.00, '2025-03-01'),
('Green Valley Apartments', 'PRJ-003', 'Thrissur, Kerala', 1, '2025-06-01', NULL, 28000000.00, '2025-06-01'),
('Technopark Phase 4', 'PRJ-004', 'Trivandrum, Kerala', 3, '2024-08-01', NULL, 75000000.00, '2024-08-01'),
('Marine Drive Promenade', 'PRJ-005', 'Kochi, Kerala', 4, '2024-01-01', '2025-02-28', 18000000.00, '2024-01-01');
GO

-- =============================================
-- 3. VENDORS
-- =============================================

INSERT INTO VND.Vendors (Name, ContactPerson, Email, Phone, Address, IsActive, CreatedAt)
VALUES
('SteelWorks India Pvt Ltd', 'Rajesh Kumar', 'rajesh@steelworks.in', '+91 98450 12345', 'Edapally, Kochi, Kerala', 1, GETUTCDATE()),
('Kerala Cement Corporation', 'Sunitha Pillai', 'sunitha@kcc.co.in', '+91 94470 23456', 'Aluva, Ernakulam, Kerala', 1, GETUTCDATE()),
('Malabar Construction Supplies', 'Mohammed Ashraf', 'ashraf@malabar.co.in', '+91 90200 34567', 'Calicut Road, Kozhikode, Kerala', 1, GETUTCDATE()),
('Tata Steel Limited', 'Anand Sharma', 'anand@tatasteel.com', '+91 98100 45678', 'Worli, Mumbai, Maharashtra', 1, GETUTCDATE()),
('Pioneer Aggregates', 'Deepa Thomas', 'deepa@pioneer.co.in', '+91 91880 56789', 'Muvattupuzha, Ernakulam, Kerala', 1, GETUTCDATE());
GO

-- =============================================
-- 4. PURCHASE REQUESTS with Items
-- For Project: Riverside Towers (Id=1)
-- =============================================

INSERT INTO PR.PurchaseRequests (ProjectId, RequestNumber, Status, RequestedBy, RequestDate, Remarks, CreatedAt)
VALUES
(1, 'PR-20250215001', 6, 'Amal Das', '2025-02-15', 'Structural steel for floors 1-5', '2025-02-15'),
(1, 'PR-20250301002', 3, 'Priya Nair', '2025-03-01', 'Cement and aggregates for foundation work', '2025-03-01'),
(2, 'PR-20250320003', 2, 'Rahul Menon', '2025-03-20', 'Steel reinforcement bars for overpass pillars', '2025-03-20');
GO

-- PR-20250215001 items (Awarded)
INSERT INTO PR.PurchaseRequestItems (PurchaseRequestId, ItemName, Unit, Quantity, Specification, CreatedAt)
VALUES
(1, 'TMT Steel Bars (Fe500)', 'MT', 45.00, '12mm diameter, IS 1786 grade', '2025-02-15'),
(1, 'Structural Steel Sections', 'MT', 12.50, 'ISMB 200, IS 2062 grade', '2025-02-15'),
(1, 'Steel Plates', 'MT', 8.00, '10mm thick, IS 2062 grade', '2025-02-15');
GO

-- PR-20250301002 items (QuotesReceived)
INSERT INTO PR.PurchaseRequestItems (PurchaseRequestId, ItemName, Unit, Quantity, Specification, CreatedAt)
VALUES
(2, 'OPC 53 Grade Cement', 'Bags', 500.00, '50kg bags, IS 269 specification', '2025-03-01'),
(2, 'Coarse Aggregate 20mm', 'CuM', 80.00, 'Crushed granite, IS 383 specification', '2025-03-01'),
(2, 'Fine Aggregate (M-Sand)', 'CuM', 60.00, 'Manufactured sand, Zone II', '2025-03-01');
GO

-- PR-20250320003 items (Submitted)
INSERT INTO PR.PurchaseRequestItems (PurchaseRequestId, ItemName, Unit, Quantity, Specification, CreatedAt)
VALUES
(3, 'TMT Steel Bars (Fe550)', 'MT', 120.00, '16mm and 20mm diameter mix', '2025-03-20'),
(3, 'Binding Wire', 'KG', 250.00, '18 gauge GI wire', '2025-03-20');
GO

-- =============================================
-- 5. VENDOR QUOTES for PR-001 (Awarded)
-- =============================================

-- Quote from SteelWorks India (VendorId=1) for PR-001
INSERT INTO VND.VendorQuotes (PurchaseRequestId, VendorId, QuoteDate, TotalAmount, IsAwarded, Remarks, CreatedAt)
VALUES
(1, 1, '2025-02-20', 3247500.00, 1, 'Includes delivery to site within 7 days', '2025-02-20');
GO

INSERT INTO VND.VendorQuoteItems (VendorQuoteId, PurchaseRequestItemId, UnitPrice, TotalPrice)
VALUES
(1, 1, 58000.00, 2610000.00),  -- TMT Steel: 45 MT x 58000
(1, 2, 62000.00, 775000.00),   -- Structural Steel: 12.5 MT x 62000
(1, 3, 60000.00, 480000.00);   -- Steel Plates: 8 MT x 60000
GO

-- Quote from Tata Steel (VendorId=4) for PR-001
INSERT INTO VND.VendorQuotes (PurchaseRequestId, VendorId, QuoteDate, TotalAmount, IsAwarded, Remarks, CreatedAt)
VALUES
(1, 4, '2025-02-21', 3442500.00, 0, 'Premium quality, IS certified', '2025-02-21');
GO

INSERT INTO VND.VendorQuoteItems (VendorQuoteId, PurchaseRequestItemId, UnitPrice, TotalPrice)
VALUES
(2, 1, 61000.00, 2745000.00),  -- TMT Steel: 45 MT x 61000
(2, 2, 65000.00, 812500.00),   -- Structural Steel: 12.5 MT x 65000
(2, 3, 61000.00, 488000.00);   -- Steel Plates: 8 MT x 61000
GO

-- Update PR-001 status to Awarded
UPDATE PR.PurchaseRequests SET Status = 6 WHERE Id = 1;
GO

-- =============================================
-- 6. VENDOR QUOTES for PR-002 (QuotesReceived)
-- =============================================

-- Quote from Kerala Cement Corporation (VendorId=2)
INSERT INTO VND.VendorQuotes (PurchaseRequestId, VendorId, QuoteDate, TotalAmount, IsAwarded, Remarks, CreatedAt)
VALUES
(2, 2, '2025-03-05', 292000.00, 0, 'Factory direct pricing, bulk discount applied', '2025-03-05');
GO

INSERT INTO VND.VendorQuoteItems (VendorQuoteId, PurchaseRequestItemId, UnitPrice, TotalPrice)
VALUES
(3, 4, 380.00, 190000.00),   -- Cement: 500 bags x 380
(3, 5, 850.00, 68000.00),    -- Coarse Aggregate: 80 CuM x 850
(3, 6, 567.00, 34000.00);    -- Fine Aggregate: 60 CuM x 567
GO

-- Quote from Malabar Construction Supplies (VendorId=3)
INSERT INTO VND.VendorQuotes (PurchaseRequestId, VendorId, QuoteDate, TotalAmount, IsAwarded, Remarks, CreatedAt)
VALUES
(2, 3, '2025-03-06', 308500.00, 0, 'Next day delivery available', '2025-03-06');
GO

INSERT INTO VND.VendorQuoteItems (VendorQuoteId, PurchaseRequestItemId, UnitPrice, TotalPrice)
VALUES
(4, 4, 395.00, 197500.00),   -- Cement: 500 bags x 395
(4, 5, 920.00, 73600.00),    -- Coarse Aggregate: 80 CuM x 920
(4, 6, 625.00, 37500.00);    -- Fine Aggregate: 60 CuM x 625
GO

-- Update PR-002 status to QuotesReceived
UPDATE PR.PurchaseRequests SET Status = 3 WHERE Id = 2;
GO

-- =============================================
-- 7. INVENTORY for Riverside Towers (ProjectId=1)
-- =============================================

INSERT INTO INV.InventoryItems (ProjectId, ItemName, Unit, QuantityInStock, MinimumStockLevel, UnitCost, CreatedAt)
VALUES
(1, 'TMT Steel Bars (Fe500) 12mm', 'MT', 28.50, 10.00, 58000.00, '2025-02-25'),
(1, 'OPC 53 Grade Cement', 'Bags', 320.00, 100.00, 380.00, '2025-03-10'),
(1, 'Coarse Aggregate 20mm', 'CuM', 45.00, 20.00, 850.00, '2025-03-10'),
(1, 'Fine Aggregate (M-Sand)', 'CuM', 8.00, 15.00, 567.00, '2025-03-10'),
(1, 'Binding Wire 18G', 'KG', 180.00, 50.00, 85.00, '2025-03-15');
GO

-- Inventory transactions history
INSERT INTO INV.InventoryTransactions (InventoryItemId, TransactionType, Quantity, TransactionDate, Reference, CreatedAt)
VALUES
-- TMT Steel received and used
(1, 1, 45.00, '2025-02-25', 'PR-20250215001', '2025-02-25'),
(1, 2, 16.50, '2025-03-10', 'Usage-Floors-1-2', '2025-03-10'),

-- Cement received and used
(2, 1, 500.00, '2025-03-10', 'PR-20250301002', '2025-03-10'),
(2, 2, 180.00, '2025-03-20', 'Foundation-Pour-1', '2025-03-20'),

-- Coarse Aggregate received and used
(3, 1, 80.00, '2025-03-10', 'PR-20250301002', '2025-03-10'),
(3, 2, 35.00, '2025-03-20', 'Foundation-Pour-1', '2025-03-20'),

-- Fine Aggregate received and used (low stock)
(4, 1, 60.00, '2025-03-10', 'PR-20250301002', '2025-03-10'),
(4, 2, 52.00, '2025-03-20', 'Foundation-Pour-1', '2025-03-20'),

-- Binding wire received
(5, 1, 180.00, '2025-03-15', 'Local-Purchase-001', '2025-03-15');
GO

-- =============================================
-- INVENTORY for NH66 Overpass (ProjectId=2)
-- =============================================

INSERT INTO INV.InventoryItems (ProjectId, ItemName, Unit, QuantityInStock, MinimumStockLevel, UnitCost, CreatedAt)
VALUES
(2, 'TMT Steel Bars (Fe550) 16mm', 'MT', 65.00, 25.00, 63000.00, '2025-03-25'),
(2, 'TMT Steel Bars (Fe550) 20mm', 'MT', 55.00, 25.00, 65000.00, '2025-03-25'),
(2, 'High Strength Concrete Mix', 'CuM', 120.00, 40.00, 4500.00, '2025-03-25');
GO

PRINT 'Sample data inserted successfully.';
PRINT 'Demo credentials:';
PRINT '  Admin: amal@constructflow.com / Demo@1234';
PRINT '  Manager: priya@constructflow.com / Demo@1234';
PRINT '  User: rahul@constructflow.com / Demo@1234';
GO
