using System.Data;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using Dapper;

namespace ConstructFlow.Infrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly DapperContext _context;

    public InventoryRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<int> CreateItemAsync(InventoryItem item)
    {
        var parameters = new DynamicParameters();
        parameters.Add("ProjectId", item.ProjectId, DbType.Int32);
        parameters.Add("ItemName", item.ItemName, DbType.String);
        parameters.Add("Unit", item.Unit, DbType.String);
        parameters.Add("MinimumStockLevel", item.MinimumStockLevel, DbType.Decimal);
        parameters.Add("UnitCost", item.UnitCost, DbType.Decimal);
        parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "INV.usp_CreateInventoryItem",
            parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("NewId");
    }

    public async Task<IEnumerable<InventoryItem>> GetByProjectIdAsync(int projectId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("ProjectId", projectId, DbType.Int32);

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<InventoryItem>(
            "INV.usp_GetInventoryByProject",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<InventoryItem?> GetByIdAsync(int id)
    {
        // Lightweight reuse pattern — fine at this scale, same tradeoff as VendorRepository.GetByIdAsync
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int32);

        return await connection.QuerySingleOrDefaultAsync<InventoryItem>(
            "SELECT Id, ProjectId, ItemName, Unit, QuantityInStock, MinimumStockLevel, UnitCost FROM INV.InventoryItems WHERE Id = @Id AND IsDeleted = 0",
            parameters);
    }

    public async Task RecordTransactionAsync(InventoryTransaction transaction)
    {
        var parameters = new DynamicParameters();
        parameters.Add("InventoryItemId", transaction.InventoryItemId, DbType.Int32);
        parameters.Add("TransactionType", (int)transaction.TransactionType, DbType.Int32);
        parameters.Add("Quantity", transaction.Quantity, DbType.Decimal);
        parameters.Add("Reference", transaction.Reference, DbType.String);
        parameters.Add("ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "INV.usp_RecordStockTransaction",
            parameters,
            commandType: CommandType.StoredProcedure);

        var returnStatus = parameters.Get<string>("ReturnStatus");
        if (returnStatus != "SUCCESS")
        {
            var errorCode = parameters.Get<string>("ErrorCode") ?? "UNKNOWN_ERROR";
            var message = errorCode switch
            {
                "INSUFFICIENT_STOCK" => "Not enough stock available for this transaction.",
                "ITEM_NOT_FOUND" => "Inventory item not found.",
                _ => "Failed to record stock transaction."
            };

            throw new ConstructFlow.Application.Common.Exceptions.BusinessRuleException(errorCode, message);
        }
    }
}