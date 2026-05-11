using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries;
using SmartPark.Application.Reservations.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.Reservations.Queries;

public class GetReservationConflictsQueryTests
{
    [Fact]
    public async Task Handle_ReturnsConflictingReservations()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var conflictInterval = new DateTimeRange(DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(4));
        var existingInterval = new DateTimeRange(DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(5));
        var newInterval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        var conflictingReservation = Reservation.Create(spaceId, userId, existingInterval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { conflictingReservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetReservationConflictsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationConflictsQuery(spaceId, newInterval);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenParkingSpaceNotFound()
    {
        var spaceId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ParkingSpace)null);

        var handler = new GetReservationConflictsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationConflictsQuery(spaceId, interval);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenNoConflicts()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingInterval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        var newInterval = new DateTimeRange(DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4));
        var reservation = Reservation.Create(spaceId, userId, existingInterval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { reservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetReservationConflictsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationConflictsQuery(spaceId, newInterval);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}