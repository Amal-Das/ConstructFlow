using System.Data;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using ConstructFlow.Domain.Enums;
using ConstructFlow.Infrastructure.Persistence;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ConstructFlow.Infrastructure.Persistence.Repositories;

public class PurchaseRequestRepository : IPurchaseRequestRepository
{
    private readonly DapperContext _context;

    public PurchaseRequestRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(PurchaseRequest request, List<PurchaseRequestItem> items)
    {
        var itemsTable = new DataTable();
        itemsTable.Columns.Add("ItemName", typeof(string));
        itemsTable.Columns.Add("Unit", typeof(string));
        itemsTable.Columns.Add("Quantity", typeof(decimal));
        itemsTable.Columns.Add("Specification", typeof(string));

        foreach (var item in items)
        {
            itemsTable.Rows.Add(item.ItemName, item.Unit, item.Quantity, item.Specification ?? (object)DBNull.Value);
        }

        var parameters = new DynamicParameters();
        parameters.Add("ProjectId", request.ProjectId, DbType.Int32);
        parameters.Add("RequestNumber", request.RequestNumber, DbType.String);
        parameters.Add("Status", (int)request.Status, DbType.Int32);
        parameters.Add("RequestedBy", request.RequestedBy, DbType.String);
        parameters.Add("RequestDate", request.RequestDate, DbType.DateTime);
        parameters.Add("Remarks", request.Remarks, DbType.String);
        parameters.Add("Items", itemsTable.AsTableValuedParameter("PR.tvp_PurchaseRequestItem"));
        parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "PR.usp_CreatePurchaseRequest",
            parameters,
            commandType: CommandType.StoredProcedure);

        var returnStatus = parameters.Get<string>("ReturnStatus");
        if (returnStatus != "SUCCESS")
        {
            var errorCode = parameters.Get<string>("ErrorCode");
            throw new InvalidOperationException($"Failed to create purchase request: {errorCode}");
        }

        return parameters.Get<int>("NewId");
    }

    public async Task<PurchaseRequest?> GetByIdWithItemsAsync(int id)
    {
        // Implemented when we build the GetById query in the next step
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PurchaseRequest>> GetByProjectIdAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        throw new NotImplementedException();
    }
}