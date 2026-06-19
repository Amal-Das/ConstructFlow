using MediatR;

namespace ConstructFlow.Application.Inventory.Commands.CreateInventoryItem;

public class CreateInventoryItemCommand : IRequest<int>
{
    public int ProjectId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitCost { get; set; }
}