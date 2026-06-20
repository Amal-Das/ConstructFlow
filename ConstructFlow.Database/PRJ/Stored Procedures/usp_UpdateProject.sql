CREATE PROCEDURE PRJ.usp_UpdateProject
    @Id             INT,
    @Name           NVARCHAR(200),
    @Location       NVARCHAR(300),
    @Status         INT,
    @EndDate        DATETIME = NULL,
    @Budget         DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE PRJ.Projects
    SET Name = @Name,
        Location = @Location,
        Status = @Status,
        EndDate = @EndDate,
        Budget = @Budget,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;
END
