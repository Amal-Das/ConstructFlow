using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages.Vendors;

public partial class VendorList
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private List<VendorDto> vendors = new();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await Http.GetFromJsonAsync<List<VendorDto>>("api/Vendors");
            vendors = result ?? new();
        }
        catch
        {
            vendors = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void GoToCreate()
    {
        Navigation.NavigateTo("vendors/create");
    }

    private class VendorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}