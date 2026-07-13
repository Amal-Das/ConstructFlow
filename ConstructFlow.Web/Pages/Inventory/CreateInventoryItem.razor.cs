using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages.Inventory;

public partial class CreateInventoryItem : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromQuery]
    public int? ProjectId { get; set; }

    private CreateInventoryItemModel model = new();
    private List<ProjectOption> projects = new();
    private string? errorMessage;
    private bool isSubmitting;

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

        if (ProjectId.HasValue)
        {
            model.ProjectId = ProjectId.Value;
        }
    }

    private async Task HandleCreate()
    {
        isSubmitting = true;
        errorMessage = null;

        try
        {
            var response = await Http.PostAsJsonAsync("api/Inventory", model);

            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("inventory");
            }
            else
            {
                errorMessage = "Failed to create inventory item.";
            }
        }
        catch
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
        Navigation.NavigateTo("inventory");
    }

    private class ProjectOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    private class CreateInventoryItemModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a project")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [MaxLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit is required")]
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Minimum stock level cannot be negative")]
        public decimal MinimumStockLevel { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than zero")]
        public decimal UnitCost { get; set; }
    }
}