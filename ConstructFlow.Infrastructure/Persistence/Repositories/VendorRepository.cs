using System.Data;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using Dapper;

namespace ConstructFlow.Infrastructure.Persistence.Repositories;

public class VendorRepository : IVendorRepository
{
    private readonly DapperContext _context;

    public VendorRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(Vendor vendor)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Name", vendor.Name, DbType.String);
        parameters.Add("ContactPerson", vendor.ContactPerson, DbType.String);
        parameters.Add("Email", vendor.Email, DbType.String);
        parameters.Add("Phone", vendor.Phone, DbType.String);
        parameters.Add("Address", vendor.Address, DbType.String);
        parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "VND.usp_CreateVendor",
            parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("NewId");
    }

    public async Task<Vendor?> GetByIdAsync(int id)
    {
        var allVendors = await GetAllAsync();
        return allVendors.FirstOrDefault(v => v.Id == id);
    }

    public async Task<IEnumerable<Vendor>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Vendor>(
            "VND.usp_GetAllVendors",
            commandType: CommandType.StoredProcedure);
    }
}