using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries;
using SmartPark.Application.Reservations.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.Reservations.Queries;

public class GetExpiredReservationsQueryTests
{
    [Fact]
    public async Task Handle_ReturnsExpiredReservations()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cutoffDate = DateTime.UtcNow;
        var expiredInterval = new DateTimeRange(DateTime.UtcNow.AddHours(-3), DateTime.UtcNow.AddHours(-1));
        var expiredReservation = Reservation.Create(spaceId, userId, expiredInterval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetExpiredReservationsAsync(cutoffDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { expiredReservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetExpiredReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetExpiredReservationsQuery(cutoffDate);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenNoExpiredReservations()
    {
        var cutoffDate = DateTime.UtcNow;
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetExpiredReservationsAsync(cutoffDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        var handler = new GetExpiredReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetExpiredReservationsQuery(cutoffDate);
        
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_IncludesSpaceDetails_InResult()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cutoffDate = DateTime.UtcNow;
        var expiredInterval = new DateTimeRange(DateTime.UtcNow.AddHours(-3), DateTime.UtcNow.AddHours(-1));
        var expiredReservation = Reservation.Create(spaceId, userId, expiredInterval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetExpiredReservationsAsync(cutoffDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { expiredReservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetExpiredReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetExpiredReservationsQuery(cutoffDate);

        var result = await handler.Handle(query, CancellationToken.None);

        var resultList = result.ToList();
        Assert.Equal("A1", resultList[0].SpaceNumber);
    }
}