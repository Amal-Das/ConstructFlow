CREATE PROCEDURE VND.usp_GetAllVendors
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, ContactPerson, Email, Phone, Address, IsActive, CreatedAt, UpdatedAt
    FROM VND.Vendors
    WHERE IsDeleted = 0
    ORDER BY Name;
END
