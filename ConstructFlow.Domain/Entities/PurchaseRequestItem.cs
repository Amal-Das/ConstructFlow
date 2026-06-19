using ConstructFlow.Domain.Common;

namespace ConstructFlow.Domain.Entities;

public class PurchaseRequestItem : BaseEntity
{
    public int PurchaseRequestId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Specification { get; set; }

    public PurchaseRequest PurchaseRequest { get; set; } = null!;
}