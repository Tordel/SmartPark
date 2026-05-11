using System.Text.Json.Serialization;
using SmartPark.Domain.Enums;

namespace SmartPark.API.Controllers;

public record PythonAnalyzeResult(
    [property: JsonPropertyName("id")]
    Guid Id,
    
    [property: JsonPropertyName("status")]
    ParkingSpaceStatus Status,
    
    [property: JsonPropertyName("confidence")]
    decimal Confidence,
    
    [property: JsonPropertyName("detected_at")]
    DateTimeOffset DetectedAt
    );