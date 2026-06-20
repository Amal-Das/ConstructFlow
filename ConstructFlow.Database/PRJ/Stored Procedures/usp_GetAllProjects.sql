CREATE PROCEDURE PRJ.usp_GetAllProjects
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
    WHERE IsDeleted = 0
    ORDER BY CreatedAt DESC;
END
