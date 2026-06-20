using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace ConstructFlow.Web.Pages;

public partial class Dashboard
{
    private bool isLoading = true;
    private int projectCount;
    private int pendingRequestCount;
    private int vendorCount;
    private int lowStockCount;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var projects = await Http.GetFromJsonAsync<List<ProjectDto>>("api/Projects");
            projectCount = projects?.Count ?? 0;

            var vendors = await Http.GetFromJsonAsync<List<VendorDto>>("api/Vendors");
            vendorCount = vendors?.Count ?? 0;

            // Pending requests and low stock need a project context for now —
            // we'll wire these up properly once we build those pages.
            // Placeholder zero values for now.
            pendingRequestCount = 0;
            lowStockCount = 0;
        }
        catch
        {
            // Swallow for now — dashboard shouldn't crash the app if API call fails
        }
        finally
        {
            isLoading = false;
        }
    }

    private class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class VendorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}