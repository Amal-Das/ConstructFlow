using ConstructFlow.Web.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ConstructFlow.Web.Layout;

public partial class MainLayout
{
    [Inject] private JwtAuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private string userName = "User";
    private string userRole = "Member";

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        userName = user.Identity?.Name ?? "User";
        userRole = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Member";
    }

    private string GetInitial()
    {
        return string.IsNullOrWhiteSpace(userName) ? "U" : userName.Trim()[0].ToString().ToUpper();
    }

    private async Task HandleLogout()
    {
        await AuthStateProvider.MarkUserAsLoggedOut();
        Navigation.NavigateTo("login", forceLoad: true);
    }
}