namespace SmartPark.Frontend.Models;

public class OccupancyRateDto
{
    public decimal OccupancyPercentage { get; set; }
    public int TotalSpaces { get; set; }
    public int OccupiedSpaces { get; set; }
    public int FreeSpaces { get; set; }
    public int ReservedSpaces { get; set; }
}