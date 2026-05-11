using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries.DTOs;

namespace SmartPark.Application.Reservations.Queries.Handlers;

public class GetReservationConflictsQueryHandler(
    IReservationsRepository repository,
    IParkingSpacesRepository parkingSpacesRepository)
    : IRequestHandler<GetReservationConflictsQuery, IEnumerable<ReservationDto>>
{
    public async Task<IEnumerable<ReservationDto>> Handle(GetReservationConflictsQuery request,
        CancellationToken cancellationToken)
    {
        var parkingSpace = await parkingSpacesRepository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);

        if (parkingSpace == null)
        {
            throw new InvalidOperationException($"Parking space {request.ParkingSpaceId} not found.");
        }

        var reservations = await repository.GetByParkingSpaceIdAsync(request.ParkingSpaceId, cancellationToken);

        var conflicts = reservations
            .Where(r => r.Interval.Start < request.Interval.End && r.Interval.End > request.Interval.Start)
            .Select(r => new ReservationDto(
                r.Id,
                r.ParkingSpaceId,
                parkingSpace.SpaceNumber,
                r.CustomerId,
                r.Interval,
                r.Status,
                r.TotalPrice,
                r.CreatedAt
            ))
            .ToList();

        return conflicts;
    }
}