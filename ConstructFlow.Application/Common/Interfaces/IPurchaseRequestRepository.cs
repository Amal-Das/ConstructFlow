using ConstructFlow.Contracts.PurchaseRequests;
using ConstructFlow.Domain.Entities;

namespace ConstructFlow.Application.Common.Interfaces;

public interface IPurchaseRequestRepository
{
    Task<int> CreateAsync(PurchaseRequest request, List<PurchaseRequestItem> items);
    Task<PurchaseRequest?> GetByIdWithItemsAsync(int id);
    Task<IEnumerable<PurchaseRequest>> GetByProjectIdAsync(int projectId);
    Task UpdateStatusAsync(int id, string status);
    Task<List<PurchaseRequestListDto>> GetAllAsync();
}