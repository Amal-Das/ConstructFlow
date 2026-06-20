CREATE PROCEDURE PR.usp_CreatePurchaseRequest
    @ProjectId      INT,
    @RequestNumber  NVARCHAR(50),
    @Status         INT,
    @RequestedBy    NVARCHAR(200),
    @RequestDate    DATETIME,
    @Remarks        NVARCHAR(500) = NULL,
    @Items          PR.tvp_PurchaseRequestItem READONLY,
    @NewId          INT OUTPUT,
    @ReturnStatus   NVARCHAR(50) OUTPUT,
    @ErrorCode      NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO PR.PurchaseRequests (ProjectId, RequestNumber, Status, RequestedBy, RequestDate, Remarks, CreatedAt)
        VALUES (@ProjectId, @RequestNumber, @Status, @RequestedBy, @RequestDate, @Remarks, GETUTCDATE());

        SET @NewId = SCOPE_IDENTITY();

        INSERT INTO PR.PurchaseRequestItems (PurchaseRequestId, ItemName, Unit, Quantity, Specification, CreatedAt)
        SELECT @NewId, ItemName, Unit, Quantity, Specification, GETUTCDATE()
        FROM @Items;

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
