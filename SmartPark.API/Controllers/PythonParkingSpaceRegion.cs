using System.Text.Json.Serialization;
using SmartPark.Domain.Enums;

namespace SmartPark.API.Controllers;

public record PythonParkingSpaceRegion(
    [property: JsonPropertyName("id")]
    Guid Id,
    
    [property: JsonPropertyName("name")]
    string Name,
    
    [property: JsonPropertyName("polygon")]
    List<List<int>> Polygon
    );