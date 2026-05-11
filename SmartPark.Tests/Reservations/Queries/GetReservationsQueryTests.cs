using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries;
using SmartPark.Application.Reservations.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.Reservations.Queries;

public class GetReservationsQueryTests
{
    [Fact]
    public async Task Handle_ReturnsUserReservations_WhenReservationsExist()
    {
        var userId = Guid.NewGuid();
        var spaceId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        var reservation = Reservation.Create(spaceId, userId, interval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByUserIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { reservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationsQuery(userId.ToString());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_FiltersOnlyActiveReservations_WhenOnlyActiveIsTrue()
    {
        var userId = Guid.NewGuid();
        var spaceId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var activeInterval = new DateTimeRange(now.AddHours(-1), now.AddHours(1));
        var pastInterval = new DateTimeRange(now.AddHours(-3), now.AddHours(-2));
        var activeReservation = Reservation.Create(spaceId, userId, activeInterval, 10.00m);
        var pastReservation = Reservation.Create(spaceId, userId, pastInterval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByUserIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { pastReservation, activeReservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationsQuery(userId.ToString(), OnlyActive: true);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenNoReservationsExist()
    {
        var userId = Guid.NewGuid();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByUserIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        var handler = new GetReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationsQuery(userId.ToString());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}