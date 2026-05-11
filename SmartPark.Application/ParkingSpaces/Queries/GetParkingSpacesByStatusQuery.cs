using MediatR;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Domain.Enums;

namespace SmartPark.Application.ParkingSpaces.Queries;

public record GetParkingSpacesByStatusQuery(
    ParkingSpaceStatus Status,
    int? MaxResults = null
    ) : IRequest<IEnumerable<ParkingSpaceDto>>;