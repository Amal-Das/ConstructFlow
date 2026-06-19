using MediatR;

namespace ConstructFlow.Application.Vendors.Commands.SubmitVendorQuote;

public class SubmitVendorQuoteCommand : IRequest<int>
{
    public int PurchaseRequestId { get; set; }
    public int VendorId { get; set; }
    public DateTime QuoteDate { get; set; }
    public string? Remarks { get; set; }
    public List<QuoteItemInputDto> Items { get; set; } = new();
}

public class QuoteItemInputDto
{
    public int PurchaseRequestItemId { get; set; }
    public decimal UnitPrice { get; set; }
}