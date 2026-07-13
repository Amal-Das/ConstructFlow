using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace ConstructFlow.Web.Pages;

public partial class Login
{
    private LoginModel loginModel = new();
    private string? errorMessage;
    private bool isLoading;

    private async Task HandleLogin()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var response = await Http.PostAsJsonAsync("api/Auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result is not null)
                {
                    await AuthStateProvider.MarkUserAsAuthenticated(result.Token);
                    Navigation.NavigateTo("");
                }
            }
            else
            {
                errorMessage = "Invalid email or password.";
            }
        }
        catch (Exception)
        {
            errorMessage = "Unable to connect to the server. Please try again.";
        }
        finally
        {
            isLoading = false;
        }
    }

    private class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}