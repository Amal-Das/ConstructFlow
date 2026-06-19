using ConstructFlow.Contracts.Vendors;
using MediatR;

namespace ConstructFlow.Application.Vendors.Queries.GetQuoteComparison;

public class GetQuoteComparisonQuery : IRequest<QuoteComparisonDto>
{
    public int PurchaseRequestId { get; set; }

    public GetQuoteComparisonQuery(int purchaseRequestId)
    {
        PurchaseRequestId = purchaseRequestId;
    }
}