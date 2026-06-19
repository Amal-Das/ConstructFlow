using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Contracts.Vendors;
using MediatR;

namespace ConstructFlow.Application.Vendors.Queries.GetQuoteComparison;

public class GetQuoteComparisonQueryHandler : IRequestHandler<GetQuoteComparisonQuery, QuoteComparisonDto>
{
    private readonly IVendorQuoteRepository _vendorQuoteRepository;

    public GetQuoteComparisonQueryHandler(IVendorQuoteRepository vendorQuoteRepository)
    {
        _vendorQuoteRepository = vendorQuoteRepository;
    }

    public async Task<QuoteComparisonDto> Handle(GetQuoteComparisonQuery request, CancellationToken cancellationToken)
    {
        return await _vendorQuoteRepository.GetComparisonDataAsync(request.PurchaseRequestId);
    }
}