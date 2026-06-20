using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ConstructFlow.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace ConstructFlow.Web.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly LocalStorageService _localStorage;
    private const string TokenKey = "authToken";

    public JwtAuthenticationStateProvider(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync(TokenKey);

        var identity = new ClaimsIdentity();

        if (!string.IsNullOrWhiteSpace(token) && !IsTokenExpired(token))
        {
            identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _localStorage.SetItemAsync(TokenKey, token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync(TokenKey);
    }

    private static bool IsTokenExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return jwt.ValidTo < DateTime.UtcNow;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}