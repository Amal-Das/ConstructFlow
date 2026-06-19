namespace ConstructFlow.Contracts.Projects;

public class CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public decimal Budget { get; set; }
}