using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartPark.Application.ParkingSpaces.Commands;
using SmartPark.Application.ParkingSpaces.Queries;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParkingSpaceController(IMediator mediator) : BaseController(mediator)
{
    //Queries
    
    [HttpGet]
    public async Task<ActionResult<PaginatedParkingSpacesDto>> GetAllParkingSpaces([FromQuery] int? pageNumber,
        [FromQuery] int? pageSize, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllParkingSpacesQuery(pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableParkingSpaces([FromQuery] SpaceType? type, [FromQuery] int? maxResults)
    {
        var query = new GetAvailableParkingSpacesQuery(null, type, maxResults);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("occupancy")]
    public async Task<ActionResult<OccupancyRateDto>> GetParkingSpaceOccupancy([FromQuery] Guid? parkingSpaceId,
        [FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken cancellationToken)
    {
        var interval = new DateTimeRange(start, end);
        var result = await _mediator.Send(new GetOccupancyRateQuery(interval, parkingSpaceId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetParkingSpaceById(Guid id)
    {
        var query = new GetParkingSpaceByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<ParkingSpaceHistoryDto>> GetParkingSpaceHistory([FromRoute] Guid id,
        [FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken cancellationToken)
    {
        var interval = new DateTimeRange(start, end);
        var result = await _mediator.Send(new GetParkingSpaceHistoryQuery(id, interval), cancellationToken);
        return Ok(result);   
    }
    
    [HttpGet("by-status/{status}")]
    public async Task<IActionResult> GetParkingSpacesByStatus(ParkingSpaceStatus status, [FromQuery] int? maxResults)
    {
        var query = new GetParkingSpacesByStatusQuery(status, maxResults);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("by-type/{type}")]
    public async Task<IActionResult> GetParkingSpacesByType(SpaceType type, [FromQuery] int? maxResults)
    {
        var query = new GetParkingSpacesByTypeQuery(type, maxResults);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetParkingSpaceStatus(Guid id)
    {
        var query = new GetParkingSpaceStatusQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    //Commands
    
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateParkingSpace([FromBody] CreateParkingSpaceCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetParkingSpaceById), new { id = result }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParkingSpace(Guid id)
    {
        await _mediator.Send(new DeleteParkingSpaceCommand(id));
        return NoContent();
    }
    
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateParkingSpaceStatus(Guid id,
        [FromBody] UpdateParkingSpaceStatusCommand command)
    {
        if (command.ParkingSpaceId != id)
            return BadRequest("ID mismatch");
        
        await _mediator.Send(command);
        return NoContent();
    }
}