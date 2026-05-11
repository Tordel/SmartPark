using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.Reservations.Queries.DTOs;

public record ReservationDto(
    Guid Id,
    Guid ParkingSpaceId,
    string SpaceNumber,
    Guid CustomerId,
    DateTimeRange Interval,
    ReservationStatus Status,
    decimal TotalPrice,
    DateTime CreatedAt
    );