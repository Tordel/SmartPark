using MediatR;

namespace SmartPark.Application.Reservations.Commands;

public record CompleteReservationCommand(
    Guid ReservationId,
    DateTime ActualEndTime,
    decimal? AdditionalFees
    )
    :IRequest<Unit>;