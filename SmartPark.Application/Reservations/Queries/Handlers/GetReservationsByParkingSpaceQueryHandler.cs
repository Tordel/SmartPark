using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries.Handlers;

public class GetReservationsByParkingSpaceQueryHandler(
    IReservationsRepository repository,
    IParkingSpacesRepository parkingSpacesRepository)
    : IRequestHandler<GetReservationsByParkingSpaceQuery, IEnumerable<ReservationDto>>
{
    public async Task<IEnumerable<ReservationDto>> Handle(GetReservationsByParkingSpaceQuery request,
        CancellationToken cancellationToken)
    {
        var parkingSpace = await parkingSpacesRepository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);

        if (parkingSpace == null)
        {
            throw new InvalidOperationException($"Parkinig space {request.ParkingSpaceId} not found.");
        }

        var reservations = await repository.GetByParkingSpaceIdAsync(request.ParkingSpaceId, cancellationToken);

        if (request.From.HasValue)
        {
            reservations = reservations.Where(r => r.Interval.Start >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            reservations = reservations.Where(r => r.Interval.End <= request.To.Value);
        }

        var dtos = reservations.Select(r => new ReservationDto(
            r.Id,
            r.ParkingSpaceId,
            parkingSpace.SpaceNumber,
            r.CustomerId,
            r.Interval,
            r.Status,
            r.TotalPrice,
            r.CreatedAt
        ));

        return dtos;
    }
}