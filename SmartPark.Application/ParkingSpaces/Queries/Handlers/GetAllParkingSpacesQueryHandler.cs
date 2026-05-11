using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;

namespace SmartPark.Application.ParkingSpaces.Queries.Handlers;

public class GetAllParkingSpacesQueryHandler(IParkingSpacesRepository repository)
    : IRequestHandler<GetAllParkingSpacesQuery, PaginatedParkingSpacesDto>
{
    public async Task<PaginatedParkingSpacesDto> Handle(GetAllParkingSpacesQuery request,
        CancellationToken cancellationToken)
    {
        var allSpaces = await repository.GetAllAsync(cancellationToken);
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 50;

        var totalCount = allSpaces.Count();
        var totalPages = (int)System.Math.Ceiling(totalCount / (decimal)pageSize);

        var items = allSpaces
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new ParkingSpaceDto(
                s.Id,
                s.SpaceNumber,
                s.Status,
                s.Type,
                s.HourlyRate,
                s.LastDetectedAt
            ))
            .ToList();

        return new PaginatedParkingSpacesDto(
            items,
            pageNumber,
            pageSize,
            totalCount,
            totalPages,
            pageNumber > 1,
            pageNumber < totalPages
        );
    }
}