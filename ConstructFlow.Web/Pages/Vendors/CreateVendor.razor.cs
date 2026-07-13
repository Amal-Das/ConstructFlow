using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages.Vendors;

public partial class CreateVendor : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private CreateVendorModel model = new();
    private string? errorMessage;
    private bool isSubmitting;

    private async Task HandleCreate()
    {
        isSubmitting = true;
        errorMessage = null;

        try
        {
            var response = await Http.PostAsJsonAsync("api/Vendors", model);

            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("vendors");
            }
            else
            {
                errorMessage = "Failed to create vendor. Please check your input.";
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
        Navigation.NavigateTo("vendors");
    }

    private class CreateVendorModel
    {
        [Required(ErrorMessage = "Vendor name is required")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact person is required")]
        [MaxLength(200)]
        public string ContactPerson { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Address { get; set; }
    }
}