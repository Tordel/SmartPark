using SmartPark.Domain.Enums;

namespace SmartPark.Application.ParkingSpaces.Queries.DTOs;

public record ParkingSpaceDto(
    Guid Id,
    string SpaceNumber,
    ParkingSpaceStatus Status,
    SpaceType Type,
    decimal HourlyRate,
    DateTime? LastDetectedAt
    );