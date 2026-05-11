using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;

namespace SmartPark.Application.ParkingSpaces.Queries.Handlers;

public class GetParkingSpacesByTypeQueryHandler(IParkingSpacesRepository repository)
    : IRequestHandler<GetParkingSpacesByTypeQuery, IEnumerable<ParkingSpaceDto>>
{
    public async Task<IEnumerable<ParkingSpaceDto>> Handle(GetParkingSpacesByTypeQuery request,
        CancellationToken cancellationToken)
    {
        var allSpaces = await repository.GetAllAsync(cancellationToken);

        var spacesByType = allSpaces
            .Where(s => s.Type == request.Type)
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
        {
            spacesByType = spacesByType.Take(request.MaxResults.Value).ToList();
        }
        
        return spacesByType;
    }
}