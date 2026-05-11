using MediatR;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.ParkingSpaces.Queries;

public record GetOccupancyRateQuery(
    DateTimeRange Interval,
    Guid? ParkingSpaceId = null
    ) : IRequest<OccupancyRateDto>;