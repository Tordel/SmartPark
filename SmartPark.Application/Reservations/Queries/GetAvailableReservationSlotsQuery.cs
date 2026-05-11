using MediatR;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries;

public record GetAvailableReservationSlotsQuery(
        Guid ParkingSpaceId,
        DateTime Date,
        int DurationMinutes = 60
    ) : IRequest<IEnumerable<AvailableSlotDto>>;