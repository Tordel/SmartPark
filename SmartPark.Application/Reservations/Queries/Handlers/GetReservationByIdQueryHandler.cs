using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries.Handlers;

public class GetReservationByIdQueryHandler(
    IReservationsRepository repository,
    IParkingSpacesRepository parkingSpacesRepository)
    : IRequestHandler<GetReservationByIdQuery, ReservationDto>
{
    public async Task<ReservationDto> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetByIdAsync(request.ReservationId, cancellationToken);

        if (reservation == null)
        {
            throw new InvalidOperationException($"Reservation {request.ReservationId} not found.");
        }

        var parkingSpace = await parkingSpacesRepository.GetByIdAsync(reservation.ParkingSpaceId, cancellationToken);

        return new ReservationDto(
            reservation.Id,
            reservation.ParkingSpaceId,
            parkingSpace.SpaceNumber,
            reservation.CustomerId,
            reservation.Interval,
            reservation.Status,
            reservation.TotalPrice,
            reservation.CreatedAt
        );
    }
}