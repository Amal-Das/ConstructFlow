using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Contracts.PurchaseRequests;
using ConstructFlow.Domain.Entities;
using ConstructFlow.Domain.Enums;
using Dapper;
using System.Data;

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
            var errorCode = parameters.Get<string>("ErrorCode") ?? "UNKNOWN_ERROR";
            throw new ConstructFlow.Application.Common.Exceptions.BusinessRuleException(
                errorCode, "Failed to create purchase request. Please check your input and try again.");
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
    public async Task<List<PurchaseRequestListDto>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        var results = await connection.QueryAsync<PurchaseRequestListFlat>(
            "PR.usp_GetAllPurchaseRequests",
            commandType: CommandType.StoredProcedure);

        return results.Select(r => new PurchaseRequestListDto
        {
            Id = r.Id,
            ProjectId = r.ProjectId,
            ProjectName = r.ProjectName,
            RequestNumber = r.RequestNumber,
            Status = ((PurchaseRequestStatus)r.Status).ToString(),
            RequestedBy = r.RequestedBy,
            RequestDate = r.RequestDate,
            Remarks = r.Remarks
        }).ToList();
    }

    private class PurchaseRequestListFlat
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string RequestNumber { get; set; } = string.Empty;
        public int Status { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string? Remarks { get; set; }
    }
}