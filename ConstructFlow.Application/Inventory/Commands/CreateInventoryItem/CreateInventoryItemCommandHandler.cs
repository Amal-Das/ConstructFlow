using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using MediatR;

namespace ConstructFlow.Application.Inventory.Commands.CreateInventoryItem;

public class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, int>
{
    private readonly IInventoryRepository _inventoryRepository;

    public CreateInventoryItemCommandHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<int> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var item = new InventoryItem
        {
            ProjectId = request.ProjectId,
            ItemName = request.ItemName,
            Unit = request.Unit,
            MinimumStockLevel = request.MinimumStockLevel,
            UnitCost = request.UnitCost,
            QuantityInStock = 0
        };

        return await _inventoryRepository.CreateItemAsync(item);
    }
}