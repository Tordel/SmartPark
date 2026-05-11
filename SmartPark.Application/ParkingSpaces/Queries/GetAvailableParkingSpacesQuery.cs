using MediatR;
using SmartPark.Domain.Enums;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;

namespace SmartPark.Application.ParkingSpaces.Queries;

public record GetAvailableParkingSpacesQuery(
    DateTime? AsOf = null,
    SpaceType? Type = null,
    int? MaxResults = null
    ) : IRequest<IEnumerable<ParkingSpaceDto>>;