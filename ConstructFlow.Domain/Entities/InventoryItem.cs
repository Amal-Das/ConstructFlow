using ConstructFlow.Domain.Common;

namespace ConstructFlow.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public int ProjectId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal QuantityInStock { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitCost { get; set; }

    public Project Project { get; set; } = null!;
    public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
}