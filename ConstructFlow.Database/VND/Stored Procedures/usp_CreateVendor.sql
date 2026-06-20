CREATE PROCEDURE VND.usp_CreateVendor
    @Name           NVARCHAR(200),
    @ContactPerson  NVARCHAR(200),
    @Email          NVARCHAR(200),
    @Phone          NVARCHAR(20),
    @Address        NVARCHAR(300) = NULL,
    @NewId          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO VND.Vendors (Name, ContactPerson, Email, Phone, Address, IsActive, CreatedAt)
    VALUES (@Name, @ContactPerson, @Email, @Phone, @Address, 1, GETUTCDATE());

    SET @NewId = SCOPE_IDENTITY();
END
