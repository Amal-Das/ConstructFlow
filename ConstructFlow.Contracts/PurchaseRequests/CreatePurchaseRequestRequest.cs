namespace ConstructFlow.Contracts.PurchaseRequests;

public class CreatePurchaseRequestRequest
{
    public int ProjectId { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public List<CreatePurchaseRequestItem> Items { get; set; } = new();
}

public class CreatePurchaseRequestItem
{
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Specification { get; set; }
}