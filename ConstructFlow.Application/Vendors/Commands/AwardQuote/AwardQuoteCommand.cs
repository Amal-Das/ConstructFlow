using MediatR;

namespace ConstructFlow.Application.Vendors.Commands.AwardQuote;

public class AwardQuoteCommand : IRequest<Unit>
{
    public int PurchaseRequestId { get; set; }
    public int VendorQuoteId { get; set; }
}