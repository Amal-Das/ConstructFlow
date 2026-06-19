using ConstructFlow.Contracts.Inventory;
using MediatR;

namespace ConstructFlow.Application.Inventory.Queries.GetInventoryByProject;

public class GetInventoryByProjectQuery : IRequest<List<InventoryItemDto>>
{
    public int ProjectId { get; set; }

    public GetInventoryByProjectQuery(int projectId)
    {
        ProjectId = projectId;
    }
}