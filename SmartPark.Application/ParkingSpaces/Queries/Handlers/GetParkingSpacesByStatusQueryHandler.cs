using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;

namespace SmartPark.Application.ParkingSpaces.Queries.Handlers;

public class GetParkingSpacesByStatusQueryHandler(IParkingSpacesRepository repository)
    : IRequestHandler<GetParkingSpacesByStatusQuery, IEnumerable<ParkingSpaceDto>>
{
    public async Task<IEnumerable<ParkingSpaceDto>> Handle(GetParkingSpacesByStatusQuery request,
        CancellationToken cancellationToken)
    {
        var allSpaces = await repository.GetAllAsync(cancellationToken);

        var spacesByStatus = allSpaces
            .Where(s => s.Status == request.Status)
            .Select(s => new ParkingSpaceDto(
                s.Id,
                s.SpaceNumber,
                s.Status,
                s.Type,
                s.HourlyRate,
                s.LastDetectedAt
            ))
            .ToList();

        if (request.MaxResults.HasValue)
            spacesByStatus = spacesByStatus.Take(request.MaxResults.Value).ToList();

        return spacesByStatus;
    }
}