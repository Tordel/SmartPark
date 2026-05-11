namespace SmartPark.Frontend.Models;

public class ParkingSpaceDto
{
    public Guid Id { get; set; }
    public string SpaceNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public DateTime? LastDetectedAt { get; set; }
    public string? Location { get; set; }
}