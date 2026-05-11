using MediatR;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.Reservations.Commands;

public record ExtendReservationCommand(
    Guid ReservationId,
    DateTimeRange NewInterval,
    string? ExtensionReason
    )
    : IRequest<Unit>;