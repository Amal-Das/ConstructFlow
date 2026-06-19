using ConstructFlow.Domain.Common;
using ConstructFlow.Domain.Enums;

namespace ConstructFlow.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public int InventoryItemId { get; set; }
    public InventoryTransactionType TransactionType { get; set; }
    public decimal Quantity { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Reference { get; set; }

    public InventoryItem InventoryItem { get; set; } = null!;
}