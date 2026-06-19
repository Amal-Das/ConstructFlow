using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using MediatR;

namespace ConstructFlow.Application.Vendors.Commands.SubmitVendorQuote;

public class SubmitVendorQuoteCommandHandler : IRequestHandler<SubmitVendorQuoteCommand, int>
{
    private readonly IVendorQuoteRepository _vendorQuoteRepository;

    public SubmitVendorQuoteCommandHandler(IVendorQuoteRepository vendorQuoteRepository)
    {
        _vendorQuoteRepository = vendorQuoteRepository;
    }

    public async Task<int> Handle(SubmitVendorQuoteCommand request, CancellationToken cancellationToken)
    {
        var quote = new VendorQuote
        {
            PurchaseRequestId = request.PurchaseRequestId,
            VendorId = request.VendorId,
            QuoteDate = request.QuoteDate,
            Remarks = request.Remarks
        };

        var items = request.Items.Select(i => new VendorQuoteItem
        {
            PurchaseRequestItemId = i.PurchaseRequestItemId,
            UnitPrice = i.UnitPrice
        }).ToList();

        return await _vendorQuoteRepository.SubmitQuoteAsync(quote, items);
    }
}