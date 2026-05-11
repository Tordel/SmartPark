using MediatR;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Domain.Enums;

namespace SmartPark.Application.ParkingSpaces.Queries;

public record GetParkingSpacesByTypeQuery(
    SpaceType Type,
    int? MaxResults = null
    ) : IRequest<IEnumerable<ParkingSpaceDto>>;