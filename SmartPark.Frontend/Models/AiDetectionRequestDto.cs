namespace SmartPark.Frontend.Models;

public class AiDetectionRequestDto
{
    [System.Text.Json.Serialization.JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("spaces")]
    public List<AiSpaceRegionDto> Spaces { get; set; } = new();

    [System.Text.Json.Serialization.JsonPropertyName("threshold")]
    public double Threshold { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("source")]
    public string? Source { get; set; }
}