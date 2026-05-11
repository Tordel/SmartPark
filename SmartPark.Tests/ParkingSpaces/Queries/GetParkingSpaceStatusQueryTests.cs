using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Application.ParkingSpaces.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Queries;

public class GetParkingSpaceStatusQueryTests
{
    [Fact]
    public async Task Handle_ReturnsParkingSpaceStatusDto_WhenSpaceExists()
    {
        var spaceId = Guid.NewGuid();
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var mockRepository = new Mock<IParkingSpacesRepository>();
        
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetParkingSpaceStatusQueryHandler(mockRepository.Object);
        var query = new GetParkingSpaceStatusQuery(spaceId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("A1", result.SpaceNumber);
        Assert.Equal(ParkingSpaceStatus.Free, result.CurrentStatus);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenSpaceNotFound()
    {
        var spaceId = Guid.NewGuid();
        var mockRepository = new Mock<IParkingSpacesRepository>();
        
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ParkingSpace)null);

        var handler = new GetParkingSpaceStatusQueryHandler(mockRepository.Object);
        var query = new GetParkingSpaceStatusQuery(spaceId);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Theory]
    [InlineData(ParkingSpaceStatus.Free)]
    [InlineData(ParkingSpaceStatus.Occupied)]
    [InlineData(ParkingSpaceStatus.Reserved)]
    public async Task Handle_WithVariousStatuses_ReturnsCorrectStatus(ParkingSpaceStatus status)
    {
        var spaceId = Guid.NewGuid();
        var parkingSpace = ParkingSpace.Create("B2", 6.00m, SpaceType.EV);
        parkingSpace.UpdateOccupancyStatus(status, DateTime.UtcNow);
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetParkingSpaceStatusQueryHandler(mockRepository.Object);
        var query = new GetParkingSpaceStatusQuery(spaceId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(status, result.CurrentStatus);
    }
}