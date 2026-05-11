using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;

namespace SmartPark.Application.ParkingSpaces.Queries.Handlers;

public class GetParkingSpaceStatusQueryHandler(IParkingSpacesRepository repository)
    : IRequestHandler<GetParkingSpaceStatusQuery, ParkingSpaceStatusDto>
{
    public async Task<ParkingSpaceStatusDto> Handle(GetParkingSpaceStatusQuery request,
        CancellationToken cancellationToken)
    {
        var space = await repository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);

        if (space == null)
            throw new InvalidOperationException($"Parking space {request.ParkingSpaceId} not found.");
        
        return new ParkingSpaceStatusDto(
            space.Id,
            space.SpaceNumber,
            space.Status,
            space.LastDetectedAt,
            DateTime.UtcNow
            );
    }
}