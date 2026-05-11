using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries.Handlers;

public class GetExpiredReservationsQueryHandler(
    IReservationsRepository repository,
    IParkingSpacesRepository parkingSpacesRepository)
    : IRequestHandler<GetExpiredReservationsQuery, IEnumerable<ReservationDto>>
{
    public async Task<IEnumerable<ReservationDto>> Handle(GetExpiredReservationsQuery request,
        CancellationToken cancellationToken)
    {
        var expiredReservations = await repository.GetExpiredReservationsAsync(request.CutoffDate, cancellationToken);

        var dtos = new List<ReservationDto>();
        foreach (var reservation in expiredReservations)
        {
            var parkingSpace =
                await parkingSpacesRepository.GetByIdAsync(reservation.ParkingSpaceId, cancellationToken);
            dtos.Add(new ReservationDto(
                reservation.Id,
                reservation.ParkingSpaceId,
                parkingSpace.SpaceNumber,
                reservation.CustomerId,
                reservation.Interval,
                reservation.Status,
                reservation.TotalPrice,
                reservation.CreatedAt
                ));
        }

        return dtos;
    }
}