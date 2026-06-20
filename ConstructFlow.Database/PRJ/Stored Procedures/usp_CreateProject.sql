CREATE PROCEDURE PRJ.usp_CreateProject
    @Name           NVARCHAR(200),
    @Code           NVARCHAR(50),
    @Location       NVARCHAR(300),
    @Status         INT,
    @StartDate      DATETIME,
    @Budget         DECIMAL(18,2),
    @NewId          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO PRJ.Projects (Name, Code, Location, Status, StartDate, Budget, CreatedAt)
    VALUES (@Name, @Code, @Location, @Status, @StartDate, @Budget, GETUTCDATE());

    SET @NewId = SCOPE_IDENTITY();
END
