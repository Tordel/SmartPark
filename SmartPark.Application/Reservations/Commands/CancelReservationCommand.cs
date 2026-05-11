using MediatR;

namespace SmartPark.Application.Reservations.Commands;

public record CancelReservationCommand(
    Guid ReservationId,
    string CancelReason,
    DateTime CancelledAt
    )
    : IRequest<Unit>;