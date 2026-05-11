using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries;
using SmartPark.Application.ParkingSpaces.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Queries;

public class GetParkingSpacesByTypeQueryTests
{
    [Fact]
    public async Task Handle_ReturnsSpacesOfSpecificType()
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

        var handler = new GetParkingSpacesByTypeQueryHandler(mockRepository.Object);
        var query = new GetParkingSpacesByTypeQuery(SpaceType.Standard);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count());
        Assert.All(result, s => Assert.Equal(SpaceType.Standard, s.Type));
    }

    [Fact]
    public async Task Handle_AppliesMaxResults_WhenSpecified()
    {
        var spaces = Enumerable.Range(1, 10)
            .Select(i => ParkingSpace.Create($"P{i}", 10.00m, SpaceType.EV))
            .ToList();
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetParkingSpacesByTypeQueryHandler(mockRepository.Object);
        var query = new GetParkingSpacesByTypeQuery(SpaceType.EV, 5);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(5, result.Count());
    }

    [Theory]
    [InlineData(SpaceType.Standard)]
    [InlineData(SpaceType.EV)]
    [InlineData(SpaceType.Disabled)]
    public async Task Handle_WithVariousTypes_ReturnsCorrectSpaces(SpaceType type)
    {
        var spaces = new List<ParkingSpace>
        {
            ParkingSpace.Create("A1", 5.00m, SpaceType.Standard),
            ParkingSpace.Create("P1", 10.00m, SpaceType.EV),
            ParkingSpace.Create("C1", 3.00m, SpaceType.Disabled)
        };
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetParkingSpacesByTypeQueryHandler(mockRepository.Object);
        var query = new GetParkingSpacesByTypeQuery(type);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotEmpty(result);
        Assert.All(result, s => Assert.Equal(type, s.Type));
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenNoSpacesOfTypeExist()
    {
        var spaces = new List<ParkingSpace>
        {
            ParkingSpace.Create("A1", 5.00m, SpaceType.Standard)
        };
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetParkingSpacesByTypeQueryHandler(mockRepository.Object);
        var query = new GetParkingSpacesByTypeQuery(SpaceType.EV);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}