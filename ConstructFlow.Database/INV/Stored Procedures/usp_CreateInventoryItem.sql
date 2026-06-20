CREATE PROCEDURE INV.usp_CreateInventoryItem
    @ProjectId          INT,
    @ItemName            NVARCHAR(200),
    @Unit                NVARCHAR(50),
    @MinimumStockLevel   DECIMAL(18,2),
    @UnitCost            DECIMAL(18,2),
    @NewId               INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO INV.InventoryItems (ProjectId, ItemName, Unit, QuantityInStock, MinimumStockLevel, UnitCost, CreatedAt)
    VALUES (@ProjectId, @ItemName, @Unit, 0, @MinimumStockLevel, @UnitCost, GETUTCDATE());

    SET @NewId = SCOPE_IDENTITY();
END
