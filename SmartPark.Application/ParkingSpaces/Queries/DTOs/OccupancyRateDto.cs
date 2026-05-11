using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.ParkingSpaces.Queries.DTOs;

public record OccupancyRateDto(
    DateTimeRange QueryRange,
    Guid? ParkingSpaceId,
    string? SpaceNumber,
    decimal OccupancyPercentage,
    int TotalSpaces,
    int AverageOccupiedSpaces,
    TimeSpan TotalOccupiedTime,
    TimeSpan TotalFreeTime,
    List<HourlyOccupancyEntry> HourlyBreakdown
    );