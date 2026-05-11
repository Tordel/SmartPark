using MediatR;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.Reservations.Commands;

public record CreateReservationCommand(
    Guid ParkingSpaceId,
    Guid UserId,
    DateTimeRange Interval,
    decimal TotalPrice
    )
    :IRequest<Guid>;