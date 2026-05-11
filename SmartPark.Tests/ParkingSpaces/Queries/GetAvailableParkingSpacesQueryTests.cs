using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries;
using SmartPark.Application.ParkingSpaces.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Queries;

public class GetAvailableParkingSpacesQueryTests
{
    [Fact]
    public async Task Handle_ReturnsFreeSpaces()
    {
        var space1 = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var space2 = ParkingSpace.Create("A2", 5.00m, SpaceType.Standard);
        space2.UpdateOccupancyStatus(ParkingSpaceStatus.Occupied, DateTime.UtcNow);
        
        var spaces = new List<ParkingSpace> { space1, space2 };
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetAvailableParkingSpacesQueryHandler(mockRepository.Object);
        var query = new GetAvailableParkingSpacesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.All(result, s => Assert.Equal(ParkingSpaceStatus.Free, s.Status));
    }

    [Fact]
    public async Task Handle_FiltersByType_WhenTypeIsSpecified()
    {
        var spaces = new List<ParkingSpace>
        {
            ParkingSpace.Create("A1", 5.00m, SpaceType.Standard),
            ParkingSpace.Create("P1", 10.00m, SpaceType.EV),
            ParkingSpace.Create("A2", 5.00m, SpaceType.Standard)
        };
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetAvailableParkingSpacesQueryHandler(mockRepository.Object);
        var query = new GetAvailableParkingSpacesQuery(Type: SpaceType.EV);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.All(result, s => Assert.Equal(SpaceType.EV, s.Type));
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

        var handler = new GetAvailableParkingSpacesQueryHandler(mockRepository.Object);
        var query = new GetAvailableParkingSpacesQuery(MaxResults: 3);
        
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenNoAvailableSpaces()
    {
        var space1 = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        space1.UpdateOccupancyStatus(ParkingSpaceStatus.Occupied, DateTime.UtcNow);
        
        var spaces = new List<ParkingSpace> { space1 };
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetAvailableParkingSpacesQueryHandler(mockRepository.Object);
        var query = new GetAvailableParkingSpacesQuery();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.Empty(result);
    }
}