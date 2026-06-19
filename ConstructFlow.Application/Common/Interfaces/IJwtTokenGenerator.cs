using ConstructFlow.Domain.Entities;

namespace ConstructFlow.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, out DateTime expiresAt);
}