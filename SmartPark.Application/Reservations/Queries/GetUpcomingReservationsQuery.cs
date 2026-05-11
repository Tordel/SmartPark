using MediatR;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries;

public record GetUpcomingReservationsQuery(
    int MaxResults = 50,
    DateTime? From = null
    ) : IRequest<IEnumerable<ReservationDto>>;