using MediatR;
using SmartPark.Application.Reservations.Queries.DTOs;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.Reservations.Queries;

public record GetReservationConflictsQuery(
    Guid ParkingSpaceId,
    DateTimeRange Interval
    ) : IRequest<IEnumerable<ReservationDto>>;