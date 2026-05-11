using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries;
using SmartPark.Application.ParkingSpaces.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Queries;

public class GetParkingSpaceHistoryQueryTests
{
    [Fact]
    public async Task Handle_ReturnsParkingSpaceHistoryDto_WhenSpaceExists()
    {
        var spaceId = Guid.NewGuid();
        var space = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var reservations = new List<Reservation>();
        var interval = new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(24));
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(space);
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservations);

        var handler = new GetParkingSpaceHistoryQueryHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var query = new GetParkingSpaceHistoryQuery(spaceId, interval);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("A1", result.SpaceNumber);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenSpaceNotFound()
    {
        var spaceId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(24));
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ParkingSpace)null);

        var handler = new GetParkingSpaceHistoryQueryHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var query = new GetParkingSpaceHistoryQuery(spaceId, interval);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_IncludesStatusHistory_InResult()
    {
        var spaceId = Guid.NewGuid();
        var space = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var reservations = new List<Reservation>();
        var interval = new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(24));
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(space);
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservations);

        var handler = new GetParkingSpaceHistoryQueryHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var query = new GetParkingSpaceHistoryQuery(spaceId, interval);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotEmpty(result.StatusHistory);
    }
}