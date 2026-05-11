using MediatR;
using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Commands;
using SmartPark.Application.ParkingSpaces.Commands.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Commands;

public class UpdateParkingSpaceStatusCommandTests
{
    [Fact]
    public async Task Handle_UpdatesStatus_WhenParkingSpaceExists()
    {
        var spaceId = Guid.NewGuid();
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);
        
        var handler = new UpdateParkingSpaceStatusCommandHandler(mockRepository.Object);
        var command = new UpdateParkingSpaceStatusCommand(spaceId, ParkingSpaceStatus.Occupied, DateTime.UtcNow);
        
        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.Equal(Unit.Value, result);
        mockRepository.Verify(r => r.UpdateAsync(parkingSpace, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenParkingSpaceNotFound()
    {
        var spaceId = Guid.NewGuid();
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ParkingSpace?)null);
        
        var handler = new UpdateParkingSpaceStatusCommandHandler(mockRepository.Object);
        var command = new UpdateParkingSpaceStatusCommand(spaceId, ParkingSpaceStatus.Occupied, DateTime.UtcNow);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Theory]
    [InlineData(ParkingSpaceStatus.Free)]
    [InlineData(ParkingSpaceStatus.Occupied)]
    [InlineData(ParkingSpaceStatus.Reserved)]
    public async Task Handle_WithVariousStatuses_UpdatesSuccessfully(ParkingSpaceStatus newStatus)
    {
        var spaceId = Guid.NewGuid();
        var parkingSpace = ParkingSpace.Create("B2", 6.00m, SpaceType.Standard);
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);
        
        var handler = new UpdateParkingSpaceStatusCommandHandler(mockRepository.Object);
        var command = new UpdateParkingSpaceStatusCommand(spaceId, newStatus, DateTime.UtcNow);
        
        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.Equal(Unit.Value, result);
        mockRepository.Verify(r => r.UpdateAsync(parkingSpace, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsRepositoryUpdate_WithDetectedAt()
    {
        var spaceId = Guid.NewGuid();
        var detectedAt = DateTime.UtcNow;
        var parkingSpace = ParkingSpace.Create("C3", 7.00m, SpaceType.Standard);
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);
        
        var handler = new UpdateParkingSpaceStatusCommandHandler(mockRepository.Object);
        var command = new UpdateParkingSpaceStatusCommand(spaceId, ParkingSpaceStatus.Occupied, detectedAt);
        
        await handler.Handle(command, CancellationToken.None);
        
        mockRepository.Verify(r => r.UpdateAsync(parkingSpace, It.IsAny<CancellationToken>()), Times.Once);
    }
}