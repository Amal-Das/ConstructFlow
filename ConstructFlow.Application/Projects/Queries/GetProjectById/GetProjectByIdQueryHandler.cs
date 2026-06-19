using AutoMapper;
using ConstructFlow.Application.Common.Exceptions;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Contracts.Projects;
using MediatR;

namespace ConstructFlow.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public GetProjectByIdQueryHandler(IProjectRepository projectRepository, IMapper mapper)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.Id);

        if (project is null)
            throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        return _mapper.Map<ProjectDto>(project);
    }
}