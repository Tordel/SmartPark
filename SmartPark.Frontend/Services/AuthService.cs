using System.Net.Http.Json;
using SmartPark.Frontend.Models;

namespace SmartPark.Frontend.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    public UserDto? CurrentUser { get; private set; }
    public event Action? OnAuthStateChanged;
    
    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/auth/me");
            if (response.IsSuccessStatusCode)
            {
                CurrentUser = await response.Content.ReadFromJsonAsync<UserDto>();
            }
            else
            {
                CurrentUser = null;
            }
        }
        catch
        {
            CurrentUser = null;
        }
        finally
        {
            OnAuthStateChanged?.Invoke();
        }
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login",
                new LoginRequestDto { Username = username, Password = password });

            if (!response.IsSuccessStatusCode) return false;

            CurrentUser = await response.Content.ReadFromJsonAsync<UserDto>();
            OnAuthStateChanged?.Invoke();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _httpClient.PostAsync("api/auth/logout", null);
        CurrentUser = null;
        OnAuthStateChanged?.Invoke();
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register",
                new LoginRequestDto { Username = username, Password = password });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    
    public bool IsAuthenticated => CurrentUser?.IsAuthenticated ?? false;
    public bool IsAdmin => CurrentUser?.IsAdmin ?? false;
}