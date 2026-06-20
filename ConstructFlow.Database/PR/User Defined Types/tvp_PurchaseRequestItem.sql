CREATE TYPE [PR].[tvp_PurchaseRequestItem] AS TABLE (
    [ItemName]      NVARCHAR (200)  NULL,
    [Unit]          NVARCHAR (50)   NULL,
    [Quantity]      DECIMAL (18, 2) NULL,
    [Specification] NVARCHAR (500)  NULL);

