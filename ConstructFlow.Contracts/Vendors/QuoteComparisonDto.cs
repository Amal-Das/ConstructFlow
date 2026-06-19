namespace ConstructFlow.Contracts.Vendors;

public class QuoteComparisonDto
{
    public int PurchaseRequestId { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public List<QuoteComparisonItemRow> ItemRows { get; set; } = new();
    public List<VendorQuoteSummary> VendorSummaries { get; set; } = new();
}

public class QuoteComparisonItemRow
{
    public int PurchaseRequestItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public List<VendorPriceCell> VendorPrices { get; set; } = new();
}

public class VendorPriceCell
{
    public int VendorId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class VendorQuoteSummary
{
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsAwarded { get; set; }
}