using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using ConstructFlow.Domain.Enums;
using MediatR;

namespace ConstructFlow.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IProjectRepository _projectRepository;

    public CreateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Name = request.Name,
            Code = request.Code,
            Location = request.Location,
            StartDate = request.StartDate,
            Budget = request.Budget,
            Status = ProjectStatus.Planning
        };

        return await _projectRepository.CreateAsync(project);
    }
}