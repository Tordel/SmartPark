namespace SmartPark.Frontend.Models;

public class PaginatedParkingSpacesDto
{
    public List<ParkingSpaceDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}