using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages.Projects;

public partial class CreateProject
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private CreateProjectModel model = new() { StartDate = DateTime.Today };
    private string? errorMessage;
    private bool isSubmitting;

    private async Task HandleCreate()
    {
        isSubmitting = true;
        errorMessage = null;

        try
        {
            var response = await Http.PostAsJsonAsync("api/Projects", model);

            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("projects");
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                errorMessage = "Failed to create project. Please check your input.";
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
        Navigation.NavigateTo("projects");
    }

    private class CreateProjectModel
    {
        [Required(ErrorMessage = "Project name is required")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Project code is required")]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [MaxLength(300)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Budget must be greater than zero")]
        public decimal Budget { get; set; }
    }
}