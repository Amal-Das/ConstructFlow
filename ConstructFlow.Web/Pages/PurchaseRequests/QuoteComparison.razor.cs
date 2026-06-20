using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages.PurchaseRequests;

public partial class QuoteComparison : ComponentBase
{
    [Parameter] public int Id { get; set; }

    [Inject] private HttpClient Http { get; set; } = default!;

    private bool isLoading = true;
    private PurchaseRequestDto? purchaseRequest;
    private List<VendorDto> vendors = new();
    private QuoteComparisonDto? comparisonData;

    private int selectedVendorId;
    private bool showQuoteForm;
    private bool isSubmittingQuote;
    private string? quoteError;
    private Dictionary<int, decimal> pendingPrices = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        try
        {
            purchaseRequest = await Http.GetFromJsonAsync<PurchaseRequestDto>($"api/PurchaseRequests/{Id}");
            vendors = await Http.GetFromJsonAsync<List<VendorDto>>("api/Vendors") ?? new();
            comparisonData = await Http.GetFromJsonAsync<QuoteComparisonDto>($"api/Vendors/comparison/{Id}");
        }
        catch
        {
            purchaseRequest = null;
        }
        finally
        {
            isLoading = false;
        }
    }

    private void OpenQuoteForm()
    {
        showQuoteForm = true;
        pendingPrices.Clear();
        quoteError = null;
    }

    private void CancelQuoteForm()
    {
        showQuoteForm = false;
        selectedVendorId = 0;
        pendingPrices.Clear();
    }

    private void SetPrice(int purchaseRequestItemId, object? value)
    {
        if (decimal.TryParse(value?.ToString(), out var price))
        {
            pendingPrices[purchaseRequestItemId] = price;
        }
    }

    private async Task SubmitQuote()
    {
        if (comparisonData is null) return;

        var items = comparisonData.ItemRows
            .Select(item => new QuoteItemInput
            {
                PurchaseRequestItemId = item.PurchaseRequestItemId,
                UnitPrice = pendingPrices.GetValueOrDefault(item.PurchaseRequestItemId, 0)
            })
            .ToList();

        if (items.Any(i => i.UnitPrice <= 0))
        {
            quoteError = "Please enter a price for every item.";
            return;
        }

        isSubmittingQuote = true;
        quoteError = null;

        try
        {
            var command = new SubmitQuoteCommand
            {
                PurchaseRequestId = Id,
                VendorId = selectedVendorId,
                QuoteDate = DateTime.Today,
                Items = items
            };

            var response = await Http.PostAsJsonAsync("api/Vendors/quotes", command);

            if (response.IsSuccessStatusCode)
            {
                showQuoteForm = false;
                selectedVendorId = 0;
                pendingPrices.Clear();
                await LoadData();
            }
            else
            {
                quoteError = "Failed to submit quote.";
            }
        }
        catch
        {
            quoteError = "Unable to connect to the server.";
        }
        finally
        {
            isSubmittingQuote = false;
        }
    }

    private async Task AwardVendor(int vendorQuoteId)
    {
        try
        {
            var command = new AwardQuoteCommand
            {
                PurchaseRequestId = Id,
                VendorQuoteId = vendorQuoteId
            };

            var response = await Http.PostAsJsonAsync("api/Vendors/award", command);

            if (response.IsSuccessStatusCode)
            {
                await LoadData();
            }
        }
        catch
        {
            // Silent fail acceptable here for now; could add a toast/error banner later
        }
    }

    private class PurchaseRequestDto
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
    }

    private class VendorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class QuoteComparisonDto
    {
        public int PurchaseRequestId { get; set; }
        public List<QuoteComparisonItemRow> ItemRows { get; set; } = new();
        public List<VendorQuoteSummary> VendorSummaries { get; set; } = new();
    }

    private class QuoteComparisonItemRow
    {
        public int PurchaseRequestItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public List<VendorPriceCell> VendorPrices { get; set; } = new();
    }

    private class VendorPriceCell
    {
        public int VendorId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    private class VendorQuoteSummary
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsAwarded { get; set; }
        public int VendorQuoteId { get; set; }
    }

    private class SubmitQuoteCommand
    {
        public int PurchaseRequestId { get; set; }
        public int VendorId { get; set; }
        public DateTime QuoteDate { get; set; }
        public List<QuoteItemInput> Items { get; set; } = new();
    }

    private class QuoteItemInput
    {
        public int PurchaseRequestItemId { get; set; }
        public decimal UnitPrice { get; set; }
    }

    private class AwardQuoteCommand
    {
        public int PurchaseRequestId { get; set; }
        public int VendorQuoteId { get; set; }
    }
}