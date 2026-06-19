using ConstructFlow.Domain.Common;
using ConstructFlow.Domain.Enums;

namespace ConstructFlow.Domain.Entities;

public class PurchaseRequest : BaseEntity
{
    public int ProjectId { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public PurchaseRequestStatus Status { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string? Remarks { get; set; }

    public Project Project { get; set; } = null!;
    public ICollection<PurchaseRequestItem> Items { get; set; } = new List<PurchaseRequestItem>();
    public ICollection<VendorQuote> VendorQuotes { get; set; } = new List<VendorQuote>();
}