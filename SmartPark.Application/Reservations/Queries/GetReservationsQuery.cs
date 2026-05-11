using MediatR;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries;

public record GetReservationsQuery(
    string UserId,
    bool? OnlyActive = null
    ) : IRequest<IEnumerable<ReservationDto>>;