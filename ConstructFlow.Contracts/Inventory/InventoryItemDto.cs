namespace ConstructFlow.Contracts.Inventory;

public class InventoryItemDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal QuantityInStock { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitCost { get; set; }
    public bool IsLowStock { get; set; }
}