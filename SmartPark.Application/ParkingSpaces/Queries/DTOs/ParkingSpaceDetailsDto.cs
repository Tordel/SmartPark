using SmartPark.Domain.Enums;

namespace SmartPark.Application.ParkingSpaces.Queries.DTOs;

public record ParkingSpaceDetailsDto(
    Guid Id,
    string SpaceNumber,
    ParkingSpaceStatus Status,
    SpaceType Type,
    decimal HourlyRate,
    DateTime? LastDetectedAt,
    int TotalReservations,
    int ActiveReservations,
    DateTime CreatedAt
    );