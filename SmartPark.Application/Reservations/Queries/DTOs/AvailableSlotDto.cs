using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.Reservations.Queries.DTOs;

public record AvailableSlotDto(
    DateTimeRange Interval,
    decimal PriceForSlot
    );