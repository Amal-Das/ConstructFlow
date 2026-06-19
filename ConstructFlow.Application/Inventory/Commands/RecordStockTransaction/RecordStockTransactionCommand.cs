using MediatR;

namespace ConstructFlow.Application.Inventory.Commands.RecordStockTransaction;

public class RecordStockTransactionCommand : IRequest<Unit>
{
    public int InventoryItemId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // StockIn, StockOut, Adjustment
    public decimal Quantity { get; set; }
    public string? Reference { get; set; }
}