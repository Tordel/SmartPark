using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries;
using SmartPark.Application.ParkingSpaces.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Queries;

public class GetAllParkingSpacesQueryTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResults_WithDefaultPageSize()
    {
        var spaces = new List<ParkingSpace>
        {
            ParkingSpace.Create("A1", 5.00m, SpaceType.Standard),
            ParkingSpace.Create("A2", 5.00m, SpaceType.Standard)
        };
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetAllParkingSpacesQueryHandler(mockRepository.Object);
        var query = new GetAllParkingSpacesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(50, result.PageSize);
    }

    [Fact]
    public async Task Handle_CalculatesTotalPages_Correctly()
    {
        var spaces = Enumerable.Range(1, 150)
            .Select(i => ParkingSpace.Create($"A{i}", 5.00m, SpaceType.Standard))
            .ToList();
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetAllParkingSpacesQueryHandler(mockRepository.Object);
        var query = new GetAllParkingSpacesQuery(1, 50);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(3, result.TotalPages);
        Assert.Equal(150, result.TotalCount);
    }

    [Fact]
    public async Task Handle_SetsPaginationFlags_Correctly()
    {
        var spaces = Enumerable.Range(1, 100)
            .Select(i => ParkingSpace.Create($"A{i}", 5.00m, SpaceType.Standard))
            .ToList();
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);

        var handler = new GetAllParkingSpacesQueryHandler(mockRepository.Object);
        var query = new GetAllParkingSpacesQuery(1, 50);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoSpacesExist()
    {
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ParkingSpace>());

        var handler = new GetAllParkingSpacesQueryHandler(mockRepository.Object);
        var query = new GetAllParkingSpacesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }
}