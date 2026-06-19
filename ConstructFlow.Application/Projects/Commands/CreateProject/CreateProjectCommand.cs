using MediatR;

namespace ConstructFlow.Application.Projects.Commands.CreateProject;

public class CreateProjectCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public decimal Budget { get; set; }
}