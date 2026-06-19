using ConstructFlow.Application.Common.Exceptions;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using ConstructFlow.Domain.Enums;
using MediatR;

namespace ConstructFlow.Application.Inventory.Commands.RecordStockTransaction;

public class RecordStockTransactionCommandHandler : IRequestHandler<RecordStockTransactionCommand, Unit>
{
    private readonly IInventoryRepository _inventoryRepository;

    public RecordStockTransactionCommandHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Unit> Handle(RecordStockTransactionCommand request, CancellationToken cancellationToken)
    {
        var item = await _inventoryRepository.GetByIdAsync(request.InventoryItemId);
        if (item is null)
            throw new NotFoundException(nameof(InventoryItem), request.InventoryItemId);

        var transaction = new InventoryTransaction
        {
            InventoryItemId = request.InventoryItemId,
            TransactionType = Enum.Parse<InventoryTransactionType>(request.TransactionType, true),
            Quantity = request.Quantity,
            TransactionDate = DateTime.UtcNow,
            Reference = request.Reference
        };

        await _inventoryRepository.RecordTransactionAsync(transaction);
        return Unit.Value;
    }
}