using ConstructFlow.Application.Common.Interfaces;
using MediatR;

namespace ConstructFlow.Application.Vendors.Commands.AwardQuote;

public class AwardQuoteCommandHandler : IRequestHandler<AwardQuoteCommand, Unit>
{
    private readonly IVendorQuoteRepository _vendorQuoteRepository;

    public AwardQuoteCommandHandler(IVendorQuoteRepository vendorQuoteRepository)
    {
        _vendorQuoteRepository = vendorQuoteRepository;
    }

    public async Task<Unit> Handle(AwardQuoteCommand request, CancellationToken cancellationToken)
    {
        await _vendorQuoteRepository.AwardQuoteAsync(request.PurchaseRequestId, request.VendorQuoteId);
        return Unit.Value;
    }
}