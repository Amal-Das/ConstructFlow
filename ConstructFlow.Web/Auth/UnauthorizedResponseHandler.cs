using Microsoft.AspNetCore.Components;
using System.Net;

namespace ConstructFlow.Web.Auth;

public class UnauthorizedResponseHandler : DelegatingHandler
{
    private readonly JwtAuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigation;

    public UnauthorizedResponseHandler(
        JwtAuthenticationStateProvider authStateProvider,
        NavigationManager navigation)
    {
        _authStateProvider = authStateProvider;
        _navigation = navigation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _authStateProvider.MarkUserAsLoggedOut();
            _navigation.NavigateTo("/login", forceLoad: true);
        }

        return response;
    }
}