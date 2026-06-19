using ConstructFlow.Contracts.Projects;
using MediatR;

namespace ConstructFlow.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQuery : IRequest<ProjectDto>
{
    public int Id { get; set; }

    public GetProjectByIdQuery(int id)
    {
        Id = id;
    }
}