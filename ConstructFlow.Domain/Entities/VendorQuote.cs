using ConstructFlow.Domain.Common;

namespace ConstructFlow.Domain.Entities;

public class VendorQuote : BaseEntity
{
    public int PurchaseRequestId { get; set; }
    public int VendorId { get; set; }
    public DateTime QuoteDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsAwarded { get; set; } = false;
    public string? Remarks { get; set; }

    public PurchaseRequest PurchaseRequest { get; set; } = null!;
    public Vendor Vendor { get; set; } = null!;
    public ICollection<VendorQuoteItem> QuoteItems { get; set; } = new List<VendorQuoteItem>();
}