using MediatR;

namespace SmartPark.Application.Reservations.Commands;

public record ReleaseExpiredReservationCommand(
    Guid ReservationId,
    DateTime ReleasedAt,
    bool GenerateLateCharges
    )
    :IRequest<Unit>;