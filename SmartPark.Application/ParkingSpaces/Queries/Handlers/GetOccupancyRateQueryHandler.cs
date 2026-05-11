using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.ParkingSpaces.Queries.Handlers;

public class GetOccupancyRateQueryHandler(
    IParkingSpacesRepository parkingSpacesRepository,
    IReservationsRepository reservationsRepository)
    : IRequestHandler<GetOccupancyRateQuery, OccupancyRateDto>
{
    public async Task<OccupancyRateDto> Handle(GetOccupancyRateQuery request, CancellationToken cancellationToken)
    {
        var allReservations = await reservationsRepository.GetAllAsync(cancellationToken);
        var allSpaces = await parkingSpacesRepository.GetAllAsync(cancellationToken);
        
        var periodReservations = allReservations
            .Where(r => r.ParkingSpaceId == request.ParkingSpaceId.Value)
            .ToList();

        if (request.ParkingSpaceId.HasValue)
        {
            periodReservations = periodReservations
                .Where(r => r.ParkingSpaceId == request.ParkingSpaceId.Value)
                .ToList();
        }
        
        var spacesForCalculation = request.ParkingSpaceId.HasValue
            ? allSpaces.Where(s => s.Id == request.ParkingSpaceId.Value).ToList()
            : allSpaces.ToList();

        var totalCapacityHours =
            spacesForCalculation.Count * (request.Interval.End - request.Interval.Start).TotalHours;
        var totalOccupiedHours = periodReservations.Sum(r => (r.Interval.End - r.Interval.Start).TotalHours);
        var occupancyPercentage = totalCapacityHours > 0 ? (decimal)(totalOccupiedHours / totalCapacityHours) * 100m : 0m;

        var hourlyBreakdown = new List<HourlyOccupancyEntry>();
        var currentHour = request.Interval.Start.Date.AddHours(request.Interval.Start.Hour);

        while (currentHour < request.Interval.End)
        {
            var nextHour = currentHour.AddHours(1);
            var hoursOccupied = periodReservations.Sum(r =>
            {
                var overlapStart = r.Interval.Start > currentHour ? r.Interval.Start : currentHour;
                var overlapEnd = r.Interval.End < nextHour ? r.Interval.End : nextHour;
                return overlapStart < overlapEnd ? (overlapEnd - overlapStart).TotalHours : 0;
            });

            var totalCapacity = spacesForCalculation.Count;
            var occupiedSpaces = (int)System.Math.Ceiling(hoursOccupied);
            var freeSpaces = totalCapacity - occupiedSpaces;
            var hourlyOccupancy = totalCapacity > 0 ? (decimal)(hoursOccupied / totalCapacity) * 100m : 0m;
            
            hourlyBreakdown.Add(new HourlyOccupancyEntry(
                currentHour,
                hourlyOccupancy,
                occupiedSpaces,
                freeSpaces
                ));
            
            currentHour = nextHour;
        }

        var spaceName = request.ParkingSpaceId.HasValue
            ? spacesForCalculation.FirstOrDefault()?.SpaceNumber
            : null;

        return new OccupancyRateDto(
            request.Interval,
            request.ParkingSpaceId,
            spaceName,
            occupancyPercentage,
            spacesForCalculation.Count,
            (int)(totalOccupiedHours / (request.Interval.End - request.Interval.Start).TotalHours),
            TimeSpan.FromHours(totalOccupiedHours),
            TimeSpan.FromHours(totalCapacityHours - totalOccupiedHours),
            hourlyBreakdown
            );
    }
}