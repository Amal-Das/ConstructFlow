using ConstructFlow.Domain.Entities;

namespace ConstructFlow.Application.Common.Interfaces;

public interface IInventoryRepository
{
    Task<int> CreateItemAsync(InventoryItem item);
    Task<IEnumerable<InventoryItem>> GetByProjectIdAsync(int projectId);
    Task<InventoryItem?> GetByIdAsync(int id);
    Task RecordTransactionAsync(InventoryTransaction transaction);
}