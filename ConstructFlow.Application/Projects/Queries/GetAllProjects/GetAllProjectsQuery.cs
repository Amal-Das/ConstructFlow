using ConstructFlow.Contracts.Projects;
using MediatR;

namespace ConstructFlow.Application.Projects.Queries.GetAllProjects;

public class GetAllProjectsQuery : IRequest<List<ProjectDto>>
{
}