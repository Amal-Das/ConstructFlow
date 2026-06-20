using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages.PurchaseRequests;

public partial class CreatePurchaseRequest
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private CreatePurchaseRequestModel model = new();
    private List<ProjectOption> projects = new();
    private string? errorMessage;
    private bool isSubmitting;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await Http.GetFromJsonAsync<List<ProjectOption>>("api/Projects");
            projects = result ?? new();
        }
        catch
        {
            projects = new();
        }
    }

    private void AddItem()
    {
        model.Items.Add(new PurchaseRequestItemModel());
    }

    private void RemoveItem(int index)
    {
        model.Items.RemoveAt(index);
    }

    private static decimal ParseDecimal(object? value)
    {
        return decimal.TryParse(value?.ToString(), out var result) ? result : 0;
    }

    private async Task HandleCreate()
    {
        if (model.ProjectId == 0)
        {
            errorMessage = "Please select a project.";
            return;
        }

        if (model.Items.Count == 0)
        {
            errorMessage = "Please add at least one item.";
            return;
        }

        if (model.Items.Any(i => string.IsNullOrWhiteSpace(i.ItemName) || string.IsNullOrWhiteSpace(i.Unit) || i.Quantity <= 0))
        {
            errorMessage = "Each item needs a name, unit, and quantity greater than zero.";
            return;
        }

        isSubmitting = true;
        errorMessage = null;

        try
        {
            var response = await Http.PostAsJsonAsync("api/PurchaseRequests", model);

            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("/purchase-requests");
            }
            else
            {
                errorMessage = "Failed to submit purchase request.";
            }
        }
        catch (Exception)
        {
            errorMessage = "Unable to connect to the server.";
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void GoBack()
    {
        Navigation.NavigateTo("/purchase-requests");
    }

    private class ProjectOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    private class CreatePurchaseRequestModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a project")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Requested by is required")]
        public string RequestedBy { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public List<PurchaseRequestItemModel> Items { get; set; } = new();
    }

    private class PurchaseRequestItemModel
    {
        public string ItemName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string? Specification { get; set; }
    }
}