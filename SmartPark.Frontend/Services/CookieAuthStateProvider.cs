using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using SmartPark.Frontend.Models;

namespace SmartPark.Frontend.Services;

public class CookieAuthStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _authService;

    public CookieAuthStateProvider(AuthService authService)
    {
        _authService = authService;
        _authService.OnAuthStateChanged += NotifyAuthStateChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_authService.CurrentUser == null || !_authService.IsAuthenticated)
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            return Task.FromResult(new AuthenticationState(anonymous));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, _authService.CurrentUser.Username),
            new Claim(ClaimTypes.Role, _authService.CurrentUser.Role)
        };
        var identity = new ClaimsIdentity(claims, "cookie");
        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }

    private void NotifyAuthStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}