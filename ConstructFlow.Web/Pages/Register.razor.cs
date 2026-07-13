using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ConstructFlow.Web.Pages;

public partial class Register
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private RegisterModel registerModel = new();
    private string? errorMessage;
    private bool isLoading;
    private bool isSuccess;

    private async Task HandleRegister()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var response = await Http.PostAsJsonAsync("api/Auth/register", registerModel);

            if (response.IsSuccessStatusCode)
            {
                isSuccess = true;
                await Task.Delay(1500);
                Navigation.NavigateTo("login");
            }
            else
            {
                var errorBody = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                errorMessage = errorBody?.Errors is { Count: > 0 } errors
                    ? string.Join(" ", errors.SelectMany(e => e.Value))
                    : errorBody?.Message ?? "Registration failed. Please try again.";
            }
        }
        catch
        {
            errorMessage = "Unable to connect to the server.";
        }
        finally
        {
            isLoading = false;
        }
    }

    private class RegisterModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }

    private class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}