using ConstructFlow.Domain.Entities;

namespace ConstructFlow.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
}