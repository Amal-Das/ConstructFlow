
CREATE PROCEDURE PR.usp_GetAllPurchaseRequests
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pr.Id,
        pr.ProjectId,
        p.Name AS ProjectName,
        pr.RequestNumber,
        pr.Status,
        pr.RequestedBy,
        pr.RequestDate,
        pr.Remarks
    FROM PR.PurchaseRequests pr
    INNER JOIN PRJ.Projects p ON p.Id = pr.ProjectId
    WHERE pr.IsDeleted = 0
    ORDER BY pr.RequestDate DESC;
END
