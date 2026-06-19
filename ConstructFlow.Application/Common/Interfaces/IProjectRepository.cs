using ConstructFlow.Domain.Entities;

namespace ConstructFlow.Application.Common.Interfaces;

public interface IProjectRepository
{
    Task<int> CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task<Project?> GetByIdAsync(int id);
    Task<IEnumerable<Project>> GetAllAsync();
}