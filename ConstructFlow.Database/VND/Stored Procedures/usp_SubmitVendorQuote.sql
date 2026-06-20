CREATE PROCEDURE VND.usp_SubmitVendorQuote
    @PurchaseRequestId  INT,
    @VendorId           INT,
    @QuoteDate          DATETIME,
    @Remarks            NVARCHAR(500) = NULL,
    @Items              VND.tvp_VendorQuoteItem READONLY,
    @NewId              INT OUTPUT,
    @ReturnStatus       NVARCHAR(50) OUTPUT,
    @ErrorCode          NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @TotalAmount DECIMAL(18,2);

        SELECT @TotalAmount = SUM(i.UnitPrice * pri.Quantity)
        FROM @Items i
        INNER JOIN PR.PurchaseRequestItems pri ON pri.Id = i.PurchaseRequestItemId;

        INSERT INTO VND.VendorQuotes (PurchaseRequestId, VendorId, QuoteDate, TotalAmount, Remarks, CreatedAt)
        VALUES (@PurchaseRequestId, @VendorId, @QuoteDate, @TotalAmount, @Remarks, GETUTCDATE());

        SET @NewId = SCOPE_IDENTITY();

        INSERT INTO VND.VendorQuoteItems (VendorQuoteId, PurchaseRequestItemId, UnitPrice, TotalPrice)
        SELECT
            @NewId,
            i.PurchaseRequestItemId,
            i.UnitPrice,
            i.UnitPrice * pri.Quantity
        FROM @Items i
        INNER JOIN PR.PurchaseRequestItems pri ON pri.Id = i.PurchaseRequestItemId;

        UPDATE PR.PurchaseRequests
        SET Status = 3 -- QuotesReceived
        WHERE Id = @PurchaseRequestId;

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
