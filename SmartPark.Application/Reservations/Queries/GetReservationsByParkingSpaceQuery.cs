using MediatR;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries;

public record GetReservationsByParkingSpaceQuery(
    Guid ParkingSpaceId,
    DateTime? From = null,
    DateTime? To = null
    ) : IRequest<IEnumerable<ReservationDto>>;