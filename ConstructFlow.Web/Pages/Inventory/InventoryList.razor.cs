using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages.Inventory;

public partial class InventoryList
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private List<ProjectOption> projects = new();
    private List<InventoryItemDto> items = new();
    private int selectedProjectId;
    private bool isLoading;

    // Transaction modal state
    private InventoryItemDto? transactionItem;
    private string transactionType = "StockIn";
    private decimal transactionQuantity;
    private string? transactionReference;
    private string? transactionError;
    private bool isSubmittingTransaction;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            projects = await Http.GetFromJsonAsync<List<ProjectOption>>("api/Projects") ?? new();
        }
        catch
        {
            projects = new();
        }
    }

    private async Task LoadInventory()
    {
        if (selectedProjectId == 0)
        {
            items = new();
            return;
        }

        isLoading = true;
        try
        {
            items = await Http.GetFromJsonAsync<List<InventoryItemDto>>($"api/Inventory/project/{selectedProjectId}") ?? new();
        }
        catch
        {
            items = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void GoToCreate()
    {
        Navigation.NavigateTo(selectedProjectId == 0
            ? "inventory/create"
            : $"inventory/create?projectId={selectedProjectId}");
    }

    private void OpenTransactionForm(InventoryItemDto item)
    {
        transactionItem = item;
        transactionType = "StockIn";
        transactionQuantity = 0;
        transactionReference = null;
        transactionError = null;
    }

    private void CloseTransactionForm()
    {
        transactionItem = null;
    }

    private async Task SubmitTransaction()
    {
        if (transactionItem is null) return;

        if (transactionQuantity <= 0)
        {
            transactionError = "Quantity must be greater than zero.";
            return;
        }

        isSubmittingTransaction = true;
        transactionError = null;

        try
        {
            var command = new RecordTransactionCommand
            {
                InventoryItemId = transactionItem.Id,
                TransactionType = transactionType,
                Quantity = transactionQuantity,
                Reference = transactionReference
            };

            var response = await Http.PostAsJsonAsync("api/Inventory/transaction", command);

            if (response.IsSuccessStatusCode)
            {
                transactionItem = null;
                await LoadInventory();
            }
            else
            {
                var errorBody = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                transactionError = errorBody?.Message ?? "Failed to record transaction.";
            }
        }
        catch
        {
            transactionError = "Unable to connect to the server.";
        }
        finally
        {
            isSubmittingTransaction = false;
        }
    }

    private class ProjectOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    private class InventoryItemDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal QuantityInStock { get; set; }
        public decimal MinimumStockLevel { get; set; }
        public decimal UnitCost { get; set; }
        public bool IsLowStock { get; set; }
    }

    private class RecordTransactionCommand
    {
        public int InventoryItemId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string? Reference { get; set; }
    }
    private class ApiErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}