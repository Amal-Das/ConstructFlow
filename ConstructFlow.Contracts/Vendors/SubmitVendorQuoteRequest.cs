namespace ConstructFlow.Contracts.Vendors;

public class SubmitVendorQuoteRequest
{
    public int PurchaseRequestId { get; set; }
    public int VendorId { get; set; }
    public DateTime QuoteDate { get; set; }
    public string? Remarks { get; set; }
    public List<QuoteItemInput> Items { get; set; } = new();
}

public class QuoteItemInput
{
    public int PurchaseRequestItemId { get; set; }
    public decimal UnitPrice { get; set; }
}