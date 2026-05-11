using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Commands;
using SmartPark.Application.ParkingSpaces.Commands.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Commands;

public class CreateParkingSpaceCommandTests
{
    [Fact]
    public async Task Handle_CreatesNewParkingSpace_WithValidInput()
    {
        var mockRepository = new Mock<IParkingSpacesRepository>();
        var handler = new CreateParkingSpaceCommandHandler(mockRepository.Object);
        var command = new CreateParkingSpaceCommand("A1", 5.00m, SpaceType.Standard);
        
        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.NotEqual(Guid.Empty, result);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<ParkingSpace>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SetsParkingSpaceProperties_Correctly()
    {
        var mockRepository = new Mock<IParkingSpacesRepository>();
        var handler = new CreateParkingSpaceCommandHandler(mockRepository.Object);
        var command = new CreateParkingSpaceCommand("B5", 7.50m, SpaceType.Standard);
        ParkingSpace? capturedSpace = null;
        
        mockRepository.Setup(r => r.AddAsync(It.IsAny<ParkingSpace>(), It.IsAny<CancellationToken>()))
            .Callback<ParkingSpace, CancellationToken>((space, ct) => capturedSpace = space);
        
        await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(capturedSpace);
        Assert.Equal("B5", capturedSpace.SpaceNumber);
        Assert.Equal(7.50m, capturedSpace.HourlyRate);
        Assert.Equal(SpaceType.Standard, capturedSpace.Type);
    }

    [Theory]
    [InlineData("A1", 5.00, SpaceType.Standard)]
    [InlineData("P1", 10.00, SpaceType.EV)]
    [InlineData("HC1", 3.00, SpaceType.Disabled)]
    public async Task Handle_WithVariousInputs_CreatesSuccessfully(string spaceNumber, decimal hourlyRate,
        SpaceType type)
    {
        var mockRepository = new Mock<IParkingSpacesRepository>();
        var handler = new CreateParkingSpaceCommandHandler(mockRepository.Object);
        var command = new CreateParkingSpaceCommand(spaceNumber, (decimal)hourlyRate, type);
        
        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.NotEqual(Guid.Empty, result);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<ParkingSpace>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsValidGuid_OnSuccess()
    {
        var mockRepository = new Mock<IParkingSpacesRepository>();
        var handler = new CreateParkingSpaceCommandHandler(mockRepository.Object);
        var command = new CreateParkingSpaceCommand("C2", 12.50m, SpaceType.Disabled);
        
        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.NotEqual(Guid.Empty, result);
    }
}