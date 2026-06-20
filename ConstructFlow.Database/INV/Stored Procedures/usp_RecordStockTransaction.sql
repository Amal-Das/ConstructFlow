CREATE PROCEDURE INV.usp_RecordStockTransaction
    @InventoryItemId    INT,
    @TransactionType    INT,
    @Quantity           DECIMAL(18,2),
    @Reference          NVARCHAR(200) = NULL,
    @ReturnStatus       NVARCHAR(50) OUTPUT,
    @ErrorCode          NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @CurrentStock DECIMAL(18,2);

        SELECT @CurrentStock = QuantityInStock
        FROM INV.InventoryItems
        WHERE Id = @InventoryItemId AND IsDeleted = 0;

        IF @CurrentStock IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SET @ReturnStatus = 'FAILURE';
            SET @ErrorCode = 'ITEM_NOT_FOUND';
            RETURN;
        END

        IF @TransactionType = 2 AND @CurrentStock < @Quantity -- StockOut guard
        BEGIN
            ROLLBACK TRANSACTION;
            SET @ReturnStatus = 'FAILURE';
            SET @ErrorCode = 'INSUFFICIENT_STOCK';
            RETURN;
        END

        INSERT INTO INV.InventoryTransactions (InventoryItemId, TransactionType, Quantity, TransactionDate, Reference, CreatedAt)
        VALUES (@InventoryItemId, @TransactionType, @Quantity, GETUTCDATE(), @Reference, GETUTCDATE());

        UPDATE INV.InventoryItems
        SET QuantityInStock = CASE
                                  WHEN @TransactionType = 1 THEN QuantityInStock + @Quantity  -- StockIn
                                  WHEN @TransactionType = 2 THEN QuantityInStock - @Quantity   -- StockOut
                                  WHEN @TransactionType = 3 THEN @Quantity                      -- Adjustment (sets absolute value)
                                  ELSE QuantityInStock
                               END,
            UpdatedAt = GETUTCDATE()
        WHERE Id = @InventoryItemId;

        IF @@ROWCOUNT = 0
        BEGIN
            ROLLBACK TRANSACTION;
            SET @ReturnStatus = 'FAILURE';
            SET @ErrorCode = 'UPDATE_FAILED';
            RETURN;
        END

        COMMIT TRANSACTION;

        SET @ReturnStatus = 'SUCCESS';
        SET @ErrorCode = NULL;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SET @ReturnStatus = 'FAILURE';
        SET @ErrorCode = ERROR_MESSAGE();
    END CATCH
END
