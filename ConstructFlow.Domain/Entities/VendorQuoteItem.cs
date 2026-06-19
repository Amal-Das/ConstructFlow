using ConstructFlow.Domain.Common;

namespace ConstructFlow.Domain.Entities;

public class VendorQuoteItem : BaseEntity
{
    public int VendorQuoteId { get; set; }
    public int PurchaseRequestItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public VendorQuote VendorQuote { get; set; } = null!;
    public PurchaseRequestItem PurchaseRequestItem { get; set; } = null!;
}