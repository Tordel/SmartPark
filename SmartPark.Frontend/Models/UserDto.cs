namespace SmartPark.Frontend.Models;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsAdmin => Role == "Admin";
    public bool IsAuthenticated => !string.IsNullOrEmpty(Username);
}