namespace SmartPark.Frontend.Models;

public class ParkingSpaceHistoryEntryDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DurationMinutes { get; set; }
}