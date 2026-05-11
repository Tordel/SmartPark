using SmartPark.Domain.Enums;

namespace SmartPark.Application.ParkingSpaces.Queries.DTOs;

public record ParkingSpaceStatusDto(
    Guid Id,
    string SpaceNumber,
    ParkingSpaceStatus CurrentStatus,
    DateTime? LastDetectedAt,
    DateTime StatusChangedAt
    );