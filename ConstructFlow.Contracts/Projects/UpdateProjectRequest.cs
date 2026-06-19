namespace ConstructFlow.Contracts.Projects;

public class UpdateProjectRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? EndDate { get; set; }
    public decimal Budget { get; set; }
}