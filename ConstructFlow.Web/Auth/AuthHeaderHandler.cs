using System.Net.Http.Headers;

namespace ConstructFlow.Web.Auth;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly JwtAuthenticationStateProvider _authStateProvider;

    public AuthHeaderHandler(JwtAuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authStateProvider.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}