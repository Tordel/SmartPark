using MediatR;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;

namespace SmartPark.Application.ParkingSpaces.Queries;

public record GetAllParkingSpacesQuery(
    int? PageNumber = 1,
    int? PageSize = 50
    ) : IRequest<PaginatedParkingSpacesDto>;