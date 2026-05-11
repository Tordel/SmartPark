using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries.DTOs;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.Reservations.Queries.Handlers;

public class GetAvailableReservationSlotsQueryHandler(
    IReservationsRepository repository,
    IParkingSpacesRepository parkingSpacesRepository)
    : IRequestHandler<GetAvailableReservationSlotsQuery, IEnumerable<AvailableSlotDto>>
{
    public async Task<IEnumerable<AvailableSlotDto>> Handle(GetAvailableReservationSlotsQuery request,
        CancellationToken cancellationToken)
    {
        var parkingSpace = await parkingSpacesRepository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);

        if (parkingSpace == null)
        {
            throw new InvalidOperationException($"Parking space {request.ParkingSpaceId} not found.");
        }

        var reservations = await repository.GetByParkingSpaceIdAsync(request.ParkingSpaceId, cancellationToken);
        var dayReservations = reservations
            .Where(r => r.Interval.Start.Date == request.Date.Date)
            .OrderBy(r => r.Interval.Start)
            .ToList();

        var availableSlots = new List<AvailableSlotDto>();
        var dayStart = request.Date.Date.AddHours(6);
        var dayEnd = request.Date.Date.AddHours(22);
        var slotDuration = TimeSpan.FromMinutes(request.DurationMinutes);
        
        var currentTime = dayStart;
        foreach (var reservation in dayReservations)
        {
            if (currentTime.AddMinutes(request.DurationMinutes) <= reservation.Interval.Start)
            {
                availableSlots.Add(new AvailableSlotDto(
                    new DateTimeRange(currentTime,currentTime.Add(slotDuration)),
                    (decimal)slotDuration.TotalHours * parkingSpace.HourlyRate));
            }
        }
        
        return availableSlots;
    }
}