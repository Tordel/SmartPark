using System.Text.Json.Serialization;

namespace SmartPark.Frontend.Models;

public class AiDetectionResultDto
{
    [JsonPropertyName("results")]
    public List<AiDetectionResultItem> Results { get; set; } = new();

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("camera_image")]
    public string? CameraImage { get; set; }
}