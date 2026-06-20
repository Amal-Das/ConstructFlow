using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace ConstructFlow.Web.Pages.Projects;

public partial class ProjectList
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    private List<ProjectDto> projects = new();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await Http.GetFromJsonAsync<List<ProjectDto>>("api/Projects");
            projects = result ?? new();
        }
        catch
        {
            projects = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void GoToCreate()
    {
        Navigation.NavigateTo("/projects/create");
    }

    private void ViewProject(int id)
    {
        Navigation.NavigateTo($"/projects/{id}");
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
        public string Code { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
    }
}