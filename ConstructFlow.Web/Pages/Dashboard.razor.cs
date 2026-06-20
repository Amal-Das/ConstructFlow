using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages;

public partial class Dashboard : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;

    private bool isLoading = true;
    private int projectCount;
    private int pendingRequestCount;
    private int vendorCount;
    private int lowStockCount;
    private List<ProjectDto> recentProjects = new();
    private List<VendorDto> vendors = new();
    private List<StatusSlice> statusBreakdown = new();

    private static readonly Dictionary<string, string> StatusColors = new()
    {
        ["Planning"] = "#94a3b8",
        ["InProgress"] = "#2563eb",
        ["OnHold"] = "#d97706",
        ["Completed"] = "#16a34a",
        ["Cancelled"] = "#dc2626"
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var projectsTask = Http.GetFromJsonAsync<List<ProjectDto>>("api/Projects");
            var vendorsTask = Http.GetFromJsonAsync<List<VendorDto>>("api/Vendors");
            var requestsTask = Http.GetFromJsonAsync<List<PurchaseRequestDto>>("api/PurchaseRequests");

            await Task.WhenAll(projectsTask, vendorsTask, requestsTask);

            var projects = projectsTask.Result ?? new();
            vendors = vendorsTask.Result ?? new();
            var requests = requestsTask.Result ?? new();

            projectCount = projects.Count(p => p.Status is "Planning" or "InProgress");
            vendorCount = vendors.Count;
            pendingRequestCount = requests.Count(r => r.Status is "Submitted" or "QuotesReceived");
            recentProjects = projects.OrderByDescending(p => p.Id).Take(5).ToList();

            BuildStatusBreakdown(projects);

            lowStockCount = 0;
            foreach (var project in projects)
            {
                try
                {
                    var inventory = await Http.GetFromJsonAsync<List<InventoryItemDto>>($"api/Inventory/project/{project.Id}");
                    lowStockCount += inventory?.Count(i => i.IsLowStock) ?? 0;
                }
                catch
                {
                    // Skip projects where inventory fetch fails
                }
            }
        }
        catch
        {
            // Leave dashboard at safe defaults if anything fails
        }
        finally
        {
            isLoading = false;
        }
    }

    private void BuildStatusBreakdown(List<ProjectDto> projects)
    {
        if (projects.Count == 0)
        {
            statusBreakdown = new();
            return;
        }

        statusBreakdown = projects
            .GroupBy(p => p.Status)
            .Select(g => new StatusSlice
            {
                Status = g.Key,
                Count = g.Count(),
                Percent = (double)g.Count() / projects.Count,
                Color = StatusColors.GetValueOrDefault(g.Key, "#94a3b8")
            })
            .OrderByDescending(s => s.Count)
            .ToList();
    }

    private const double CircleCircumference = 2 * Math.PI * 45; // r=45 from the SVG

    private string GetDashArray(double percent)
    {
        var length = percent * CircleCircumference;
        return $"{length} {CircleCircumference}";
    }

    private string GetDashOffset(double cumulativePercentBefore)
    {
        return (-(cumulativePercentBefore * CircleCircumference)).ToString();
    }

    private static string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        return hour switch
        {
            < 12 => "Good morning",
            < 17 => "Good afternoon",
            _ => "Good evening"
        };
    }

    private static string GetStatusClass(string status) => status switch
    {
        "Planning" => "neutral",
        "InProgress" => "info",
        "OnHold" => "warning",
        "Completed" => "success",
        "Cancelled" => "danger",
        _ => "neutral"
    };

    private class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    private class VendorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    private class PurchaseRequestDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    private class InventoryItemDto
    {
        public bool IsLowStock { get; set; }
    }

    private class StatusSlice
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percent { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}