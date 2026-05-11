using System.Text.Json.Serialization;
using SmartPark.Domain.Enums;

namespace SmartPark.API.Controllers;

public record PythonAnalyzeRequest(
    [property: JsonPropertyName("image_url")]
    string ImageUrl,

    [property: JsonPropertyName("spaces")] List<PythonParkingSpaceRegion> Spaces,

    [property: JsonPropertyName("threshold")]
    double? Threshold,

    [property: JsonPropertyName("source")] string? Source
    );