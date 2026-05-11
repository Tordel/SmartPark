using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.ParkingSpaces.Queries.DTOs;

public record ParkingSpaceHistoryDto(
    Guid ParkingSpaceId,
    string SpaceNumber,
    DateTimeRange QueryRange,
    List<SpaceStatusHistoryEntry> StatusHistory,
    TimeSpan TotalOccupiedTime,
    TimeSpan TotalFreeTime,
    decimal OccupancyPercentage
    );