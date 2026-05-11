using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries;
using SmartPark.Application.ParkingSpaces.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Queries;

public class GetParkingSpacesByStatusQueryTests
{
    [Fact]
    public async Task Handle_ReturnsSpacesWithSpecificStatus()
    {
        var space1 = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var space2 = ParkingSpace.Create("A2", 5.00m, SpaceType.Standard);
        space2.UpdateOccupancyStatus(ParkingSpaceStatus.Occupied, DateTime.UtcNow);
        
        var spaces = new List<ParkingSpace> { space1, space2 };
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetParkingSpacesByStatusQueryHandler(mockRepository.Object);
        var query = new GetParkingSpacesByStatusQuery(ParkingSpaceStatus.Free);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.All(result, s => Assert.Equal(ParkingSpaceStatus.Free, s.Status));
    }

    [Fact]
    public async Task Handle_AppliesMaxResults_WhenSpecified()
    {
        var spaces = Enumerable.Range(1, 10)
            .Select(i => ParkingSpace.Create($"A{i}", 5.00m, SpaceType.Standard))
            .ToList();
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetParkingSpacesByStatusQueryHandler(mockRepository.Object);
        var query = new GetParkingSpacesByStatusQuery(ParkingSpaceStatus.Free, 5);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(5, result.Count());
    }

    [Theory]
    [InlineData(ParkingSpaceStatus.Free)]
    [InlineData(ParkingSpaceStatus.Occupied)]
    [InlineData(ParkingSpaceStatus.Reserved)]
    public async Task Handle_WithVariousStatuses_ReturnsCorrectSpaces(ParkingSpaceStatus status)
    {
        var space1 = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var space2 = ParkingSpace.Create("A2", 5.00m, SpaceType.Standard);
        space2.UpdateOccupancyStatus(status, DateTime.UtcNow);
        
        var spaces = new List<ParkingSpace> { space1, space2 };
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetParkingSpacesByStatusQueryHandler(mockRepository.Object);
        var query = new GetParkingSpacesByStatusQuery(status);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotEmpty(result);
        Assert.All(result, s => Assert.Equal(status, s.Status));
    }
}