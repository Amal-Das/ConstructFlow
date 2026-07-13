using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages.PurchaseRequests;

public partial class PurchaseRequestList : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private List<PurchaseRequestListDto> requests = new();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await Http.GetFromJsonAsync<List<PurchaseRequestListDto>>("api/PurchaseRequests");
            requests = result ?? new();
        }
        catch
        {
            requests = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void GoToCreate()
    {
        Navigation.NavigateTo("purchase-requests/create");
    }

    private void ViewComparison(int id)
    {
        Navigation.NavigateTo($"purchase-requests/{id}/comparison");
    }

    private static string GetStatusClass(string status) => status switch
    {
        "Draft" => "neutral",
        "Submitted" => "info",
        "QuotesReceived" => "warning",
        "Approved" => "success",
        "Rejected" => "danger",
        "Awarded" => "success",
        _ => "neutral"
    };

    private static string FormatStatus(string status)
    {
        // Inserts a space before each capital letter: "QuotesReceived" -> "Quotes Received"
        return Regex.Replace(status, "(?<!^)([A-Z])", " $1");
    }

    private class PurchaseRequestListDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string RequestNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string? Remarks { get; set; }
    }
}