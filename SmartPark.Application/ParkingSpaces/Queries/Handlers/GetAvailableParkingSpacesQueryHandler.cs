using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Domain.Enums;

namespace SmartPark.Application.ParkingSpaces.Queries.Handlers;

public class GetAvailableParkingSpacesQueryHandler(IParkingSpacesRepository repository)
    : IRequestHandler<GetAvailableParkingSpacesQuery, IEnumerable<ParkingSpaceDto>>
{
    public async Task<IEnumerable<ParkingSpaceDto>> Handle(GetAvailableParkingSpacesQuery request,
        CancellationToken cancellationToken)
    {
        var allSpaces = await repository.GetAllAsync(cancellationToken);

        var availableSpaces = allSpaces
            .Where(s => s.Status == ParkingSpaceStatus.Free);

        if (request.Type.HasValue)
        {
            availableSpaces = availableSpaces.Where(s => s.Type == request.Type.Value);
        }

        var dtos = availableSpaces
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
            dtos = dtos.Take(request.MaxResults.Value).ToList();
        }
        
        return dtos;
    }
}