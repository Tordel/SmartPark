namespace SmartPark.Frontend.Models;

public class ParkingSpaceHistoryDto
{
    public Guid ParkingSpaceId { get; set; }
    public List<ParkingSpaceHistoryEntryDto> Entries { get; set; } = new();
}