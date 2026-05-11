using System.Text.Json.Serialization;

namespace SmartPark.Frontend.Models;

public class AiDetectionResultItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("detected_at")]
    public DateTime DetectedAt { get; set; }
}