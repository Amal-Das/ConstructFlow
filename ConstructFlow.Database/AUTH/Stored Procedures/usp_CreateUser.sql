
CREATE PROCEDURE AUTH.usp_CreateUser
    @FullName       NVARCHAR(200),
    @Email          NVARCHAR(200),
    @PasswordHash   NVARCHAR(500),
    @Role           NVARCHAR(50),
    @NewId          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AUTH.Users (FullName, Email, PasswordHash, Role, IsActive, CreatedAt)
    VALUES (@FullName, @Email, @PasswordHash, @Role, 1, GETUTCDATE());

    SET @NewId = SCOPE_IDENTITY();
END
