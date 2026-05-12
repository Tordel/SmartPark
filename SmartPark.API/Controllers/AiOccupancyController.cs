using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPark.Application.ParkingSpaces.Commands;
using SmartPark.Domain.Enums;

namespace SmartPark.API.Controllers;

[ApiController]
[Route("api/ai/occupancy")]
[Authorize(Roles = "Admin")]
public class AiOccupancyController(IMediator mediator, IConfiguration configuration, IHttpClientFactory httpClientFactory) : ControllerBase
{
    private static readonly JsonSerializerOptions PythonJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
    
    [HttpPost("analyze")]
    public async Task<ActionResult<PythonAnalyzeResponse>> AnalyzeWithPythonAi(
        [FromBody] PythonAnalyzeRequest request,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient("PythonAi");
        var endpoint = configuration["AiIntegration:AnalyzeEndpoint"] ?? "/analyze-local";
        
        var response = await client.PostAsJsonAsync(endpoint, request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);

            return StatusCode(
                (int)response.StatusCode,
                new
                {
                    message = "Python AI service failed.",
                    statusCode = (int)response.StatusCode,
                    details = errorBody
                });
        }
        
        var aiResponse = await response.Content.ReadFromJsonAsync<PythonAnalyzeResponse>(PythonJsonOptions, cancellationToken);

        if (aiResponse is null)
        {
            return StatusCode(502, "Python AI service returned an empty or invalid response.");
        }

        foreach (var result in aiResponse.Results)
        {
            var command = new UpdateParkingSpaceStatusFromAiCommand(
                result.Id,
                result.Status,
                result.DetectedAt.UtcDateTime,
                result.Confidence,
                aiResponse.Source ?? request.Source
                );
            
            await mediator.Send(command, cancellationToken);
        }
        
        return Ok(aiResponse);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateParkingSpaceStatus([FromHeader(Name = "X-API-KEY")] string apiKey,
        [FromBody] AiOccupancyUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var expectedKey = configuration["AiIntegration:ApiKey"];
        
        if (string.IsNullOrWhiteSpace(expectedKey) || apiKey != expectedKey)
            return Unauthorized();
        
        var command = new UpdateParkingSpaceStatusFromAiCommand(
            request.Id,
            request.NewStatus,
            request.DetectedAt,
            request.Confidence,
            request.Source
        );
        
        await mediator.Send(command, cancellationToken);
        
        return NoContent();
    }

    [HttpPost("batch")]
    public async Task<IActionResult> UpdateBatch([FromHeader(Name = "X-API-KEY")] string apiKey,
        [FromBody] AiOccupancyBatchRequest request,
        CancellationToken cancellationToken)
    {
        
        var expectedKey = configuration["AiIntegration:ApiKey"];
        
        if (string.IsNullOrWhiteSpace(expectedKey) || apiKey != expectedKey)
            return Unauthorized();
        
        foreach (var item in request.Detections)
        {
            var command = new UpdateParkingSpaceStatusFromAiCommand(
                item.Id,
                item.NewStatus,
                request.DetectedAt,
                item.Confidence,
                request.Source
            );
            
            await mediator.Send(command, cancellationToken);
        }

        return NoContent();
    }
}

public record AiOccupancyUpdateRequest(
    [property: JsonRequired]
    Guid Id,
    [property: JsonRequired]
    ParkingSpaceStatus NewStatus,
    DateTime? DetectedAt,
    decimal? Confidence,
    string? Source
    );

public record AiOccupancyBatchRequest(
    DateTime? DetectedAt,
    string? Source,
    List<AiOccupancyBatchItem> Detections
    );
    
public record AiOccupancyBatchItem(
    Guid Id,
    ParkingSpaceStatus NewStatus,
    decimal? Confidence
    );