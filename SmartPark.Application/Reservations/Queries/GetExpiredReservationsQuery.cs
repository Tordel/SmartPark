using MediatR;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries;

public record GetExpiredReservationsQuery(
    DateTime CutoffDate
    ) : IRequest<IEnumerable<ReservationDto>>;