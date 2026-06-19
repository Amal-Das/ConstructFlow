using ConstructFlow.Application.Common.Exceptions;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Enums;
using MediatR;

namespace ConstructFlow.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Unit> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.Id);

        if (project is null)
            throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        project.Name = request.Name;
        project.Location = request.Location;
        project.Status = Enum.Parse<ProjectStatus>(request.Status, true);
        project.EndDate = request.EndDate;
        project.Budget = request.Budget;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);

        return Unit.Value;
    }
}