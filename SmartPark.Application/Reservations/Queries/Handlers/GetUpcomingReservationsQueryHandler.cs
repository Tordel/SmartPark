using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries.Handlers;

public class GetUpcomingReservationsQueryHandler(
    IReservationsRepository repository,
    IParkingSpacesRepository parkingSpacesRepository)
    : IRequestHandler<GetUpcomingReservationsQuery, IEnumerable<ReservationDto>>
{
    public async Task<IEnumerable<ReservationDto>> Handle(GetUpcomingReservationsQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await repository.GetActiveReservationsAsync(cancellationToken);
        var from = request.From ?? DateTime.UtcNow;
        
        var upcomingReservations = reservations
            .Where(r => r.Interval.Start >= from)
            .OrderBy(r => r.Interval.Start)
            .Take(request.MaxResults)
            .ToList();

        var dtos = new List<ReservationDto>();
        foreach (var reservation in upcomingReservations)
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