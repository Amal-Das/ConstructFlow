CREATE PROCEDURE INV.usp_GetInventoryByProject
    @ProjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        ProjectId,
        ItemName,
        Unit,
        QuantityInStock,
        MinimumStockLevel,
        UnitCost
    FROM INV.InventoryItems
    WHERE ProjectId = @ProjectId
      AND IsDeleted = 0
    ORDER BY ItemName;
END
