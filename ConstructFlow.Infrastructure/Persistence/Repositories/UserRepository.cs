using System.Data;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using ConstructFlow.Infrastructure.Persistence;
using Dapper;

namespace ConstructFlow.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Email", email, DbType.String);

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "AUTH.usp_GetUserByEmail",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(User user)
    {
        var parameters = new DynamicParameters();
        parameters.Add("FullName", user.FullName, DbType.String);
        parameters.Add("Email", user.Email, DbType.String);
        parameters.Add("PasswordHash", user.PasswordHash, DbType.String);
        parameters.Add("Role", user.Role, DbType.String);
        parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "AUTH.usp_CreateUser",
            parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("NewId");
    }
}