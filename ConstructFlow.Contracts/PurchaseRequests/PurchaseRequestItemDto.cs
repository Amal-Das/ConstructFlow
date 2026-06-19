namespace ConstructFlow.Contracts.PurchaseRequests;

public class PurchaseRequestItemDto
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Specification { get; set; }
}