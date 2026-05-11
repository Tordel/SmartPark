using MediatR;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries;

public record GetReservationByIdQuery(
        Guid ReservationId
    ) : IRequest<ReservationDto>;