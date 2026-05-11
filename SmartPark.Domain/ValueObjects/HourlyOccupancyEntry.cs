namespace SmartPark.Domain.ValueObjects;

public record HourlyOccupancyEntry
{
    private DateTime Hour { get; init; }
    private decimal OccupancyPercentage { get; init; }
    private int OccupiedSpaces { get; init; }
    private int FreeSpaces { get; init; }

    public HourlyOccupancyEntry(DateTime hour, decimal occupancyPercentage, int occupiedSpaces, int freeSpaces)
    {
        Hour = hour;
        OccupancyPercentage = occupancyPercentage;
        OccupiedSpaces = occupiedSpaces;
        FreeSpaces = freeSpaces;
    }
    
    private HourlyOccupancyEntry() { }
};