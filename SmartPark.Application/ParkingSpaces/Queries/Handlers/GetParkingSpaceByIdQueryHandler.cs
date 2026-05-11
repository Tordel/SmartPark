using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;

namespace SmartPark.Application.ParkingSpaces.Queries.Handlers;

public class GetParkingSpaceByIdQueryHandler(
    IParkingSpacesRepository repository,
    IReservationsRepository reservationsRepository)
    : IRequestHandler<GetParkingSpaceByIdQuery, ParkingSpaceDetailsDto>
{
    public async Task<ParkingSpaceDetailsDto> Handle(GetParkingSpaceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var space = await repository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);

        if (space == null)
            throw new InvalidOperationException($"Parking space {request.ParkingSpaceId} not found.");

        var reservations =
            await reservationsRepository.GetByParkingSpaceIdAsync(request.ParkingSpaceId, cancellationToken);
        var activeReservations = reservations
            .Count(r => r.Interval.Start <= DateTime.UtcNow && r.Interval.End >= DateTime.UtcNow);

        return new ParkingSpaceDetailsDto(
            space.Id,
            space.SpaceNumber,
            space.Status,
            space.Type,
            space.HourlyRate,
            space.LastDetectedAt,
            reservations.Count(),
            activeReservations,
            DateTime.UtcNow
        );
    }
}