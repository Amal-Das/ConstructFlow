CREATE PROCEDURE VND.usp_GetQuoteComparisonData
    @PurchaseRequestId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: PR Items (rows)
    SELECT Id AS PurchaseRequestItemId, ItemName, Quantity
    FROM PR.PurchaseRequestItems
    WHERE PurchaseRequestId = @PurchaseRequestId
      AND IsDeleted = 0;

    -- Result Set 2: Vendor quote summaries (columns)
    SELECT
        vq.Id AS VendorQuoteId,
        vq.VendorId,
        v.Name AS VendorName,
        vq.TotalAmount,
        vq.IsAwarded
    FROM VND.VendorQuotes vq
    INNER JOIN VND.Vendors v ON v.Id = vq.VendorId
    WHERE vq.PurchaseRequestId = @PurchaseRequestId
      AND vq.IsDeleted = 0;

    -- Result Set 3: Price cells (item x vendor grid data)
    SELECT
        vqi.PurchaseRequestItemId,
        vq.VendorId,
        vqi.UnitPrice,
        vqi.TotalPrice
    FROM VND.VendorQuoteItems vqi
    INNER JOIN VND.VendorQuotes vq ON vq.Id = vqi.VendorQuoteId
    WHERE vq.PurchaseRequestId = @PurchaseRequestId
      AND vq.IsDeleted = 0;
END
