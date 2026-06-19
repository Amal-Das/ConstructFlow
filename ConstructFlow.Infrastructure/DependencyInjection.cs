using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Infrastructure.Auth;
using ConstructFlow.Infrastructure.Persistence;
using ConstructFlow.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConstructFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DapperContext>();

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}