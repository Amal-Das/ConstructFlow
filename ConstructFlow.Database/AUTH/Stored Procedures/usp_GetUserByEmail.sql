
CREATE PROCEDURE AUTH.usp_GetUserByEmail
    @Email NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        FullName,
        Email,
        PasswordHash,
        Role,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM AUTH.Users
    WHERE Email = @Email
      AND IsDeleted = 0;
END
