using System.Data;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using ConstructFlow.Domain.Enums;
using Dapper;

namespace ConstructFlow.Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly DapperContext _context;

    public ProjectRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(Project project)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Name", project.Name, DbType.String);
        parameters.Add("Code", project.Code, DbType.String);
        parameters.Add("Location", project.Location, DbType.String);
        parameters.Add("Status", (int)project.Status, DbType.Int32);
        parameters.Add("StartDate", project.StartDate, DbType.DateTime);
        parameters.Add("Budget", project.Budget, DbType.Decimal);
        parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "PRJ.usp_CreateProject",
            parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("NewId");
    }

    public async Task UpdateAsync(Project project)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", project.Id, DbType.Int32);
        parameters.Add("Name", project.Name, DbType.String);
        parameters.Add("Location", project.Location, DbType.String);
        parameters.Add("Status", (int)project.Status, DbType.Int32);
        parameters.Add("EndDate", project.EndDate, DbType.DateTime);
        parameters.Add("Budget", project.Budget, DbType.Decimal);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "PRJ.usp_UpdateProject",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int32);

        using var connection = _context.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<ProjectFlat>(
            "PRJ.usp_GetProjectById",
            parameters,
            commandType: CommandType.StoredProcedure);

        return result?.ToEntity();
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        var results = await connection.QueryAsync<ProjectFlat>(
            "PRJ.usp_GetAllProjects",
            commandType: CommandType.StoredProcedure);

        return results.Select(r => r.ToEntity());
    }

    private class ProjectFlat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Project ToEntity() => new()
        {
            Id = Id,
            Name = Name,
            Code = Code,
            Location = Location,
            Status = (ProjectStatus)Status,
            StartDate = StartDate,
            EndDate = EndDate,
            Budget = Budget,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }
}