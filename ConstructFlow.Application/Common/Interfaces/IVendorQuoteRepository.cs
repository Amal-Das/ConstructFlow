using ConstructFlow.Contracts.Vendors;
using ConstructFlow.Domain.Entities;

namespace ConstructFlow.Application.Common.Interfaces;

public interface IVendorQuoteRepository
{
    Task<int> SubmitQuoteAsync(VendorQuote quote, List<VendorQuoteItem> items);
    Task<QuoteComparisonDto> GetComparisonDataAsync(int purchaseRequestId);
    Task AwardQuoteAsync(int purchaseRequestId, int vendorQuoteId);
}