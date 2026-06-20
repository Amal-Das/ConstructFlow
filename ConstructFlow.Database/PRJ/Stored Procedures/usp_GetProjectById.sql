CREATE PROCEDURE PRJ.usp_GetProjectById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        Code,
        Location,
        Status,
        StartDate,
        EndDate,
        Budget,
        CreatedAt,
        UpdatedAt
    FROM PRJ.Projects
    WHERE Id = @Id
      AND IsDeleted = 0;
END
