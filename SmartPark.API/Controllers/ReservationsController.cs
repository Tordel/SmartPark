using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartPark.Application.Reservations.Commands;
using SmartPark.Application.Reservations.Queries;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(IMediator mediator) : BaseController(mediator)
{
    // Queries

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableReservations(Guid parkingSpaceId, [FromQuery] DateTime date,
        [FromQuery] int durationMinutes = 60)
    {
        var query = new GetAvailableReservationSlotsQuery(parkingSpaceId, date, durationMinutes);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredReservations([FromQuery] DateTime cutoffDate)
    {
        var query = new GetExpiredReservationsQuery(cutoffDate);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservationById(Guid id)
    {
        var query = new GetReservationByIdQuery(id);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpGet("conflicts")]
    public async Task<IActionResult> GetConflictingReservations([FromQuery] Guid parkingSpaceId,
        [FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var interval = new DateTimeRange(start, end);
        var query = new GetReservationConflictsQuery(parkingSpaceId, interval);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-parking-space/{parkingSpaceId}")]
    public async Task<IActionResult> GetReservationsByParkingSpace(Guid parkingSpaceId)
    {
        var query = new GetReservationsByParkingSpaceQuery(parkingSpaceId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReservations(string userId, [FromQuery] bool? onlyActive)
    {
        var query = new GetReservationsQuery(userId, onlyActive);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingReservations([FromQuery] DateTime? from, [FromQuery] int maxResults)
    {
        var query = new GetUpcomingReservationsQuery(maxResults, from);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    // Commands
    
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelReservation(Guid id, [FromBody] CancelReservationCommand command)
    {
        if (command.ReservationId != id)
            return BadRequest("ID mismatch");
        
        await _mediator.Send(command);
        return NoContent();
    }
    
    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteReservation(Guid id, [FromBody] CompleteReservationCommand command)
    {
        if (command.ReservationId != id)
            return BadRequest("ID mismatch");
        
        await _mediator.Send(command);
        return NoContent();   
    }
    
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateReservation([FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetReservationById), new { id = result }, result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> ExtendReservationCommand(Guid id, [FromBody] ExtendReservationCommand command)
    {
        if (command.ReservationId != id)
            return BadRequest("ID mismatch");
        
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{id}/release")]
    public async Task<IActionResult> ReleaseExpiredReservationCommand(Guid id,
        [FromBody] ReleaseExpiredReservationCommand command)
    {
        if (command.ReservationId != id)
            return BadRequest("ID mismatch");
        
        await _mediator.Send(command);
        return NoContent();
    }
}