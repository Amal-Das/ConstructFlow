CREATE PROCEDURE VND.usp_AwardQuote
    @PurchaseRequestId INT,
    @VendorQuoteId      INT,
    @ReturnStatus       NVARCHAR(50) OUTPUT,
    @ErrorCode          NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Reset all quotes for this PR first
        UPDATE VND.VendorQuotes
        SET IsAwarded = 0
        WHERE PurchaseRequestId = @PurchaseRequestId;

        -- Award the selected one
        UPDATE VND.VendorQuotes
        SET IsAwarded = 1
        WHERE Id = @VendorQuoteId
          AND PurchaseRequestId = @PurchaseRequestId;

        IF @@ROWCOUNT = 0
        BEGIN
            ROLLBACK TRANSACTION;
            SET @ReturnStatus = 'FAILURE';
            SET @ErrorCode = 'QUOTE_NOT_FOUND';
            RETURN;
        END

        UPDATE PR.PurchaseRequests
        SET Status = 6 -- Awarded
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
