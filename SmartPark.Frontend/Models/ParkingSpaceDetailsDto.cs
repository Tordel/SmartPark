namespace SmartPark.Frontend.Models;

public class ParkingSpaceDetailsDto : ParkingSpaceDto
{
    public List<ParkingSpaceHistoryEntryDto> History { get; set; } = new();
    public decimal OccupancyPercentage { get; set; }
}