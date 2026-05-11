using System.Text.Json.Serialization;

namespace SmartPark.Frontend.Models;

public class AiSpaceRegionDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("polygon")] public List<List<int>> Polygon { get; set; } = new();
}