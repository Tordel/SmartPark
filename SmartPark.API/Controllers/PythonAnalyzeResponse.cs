using System.Text.Json.Serialization;

namespace SmartPark.API.Controllers;

public record PythonAnalyzeResponse(
    [property: JsonPropertyName("camera_image")]
    string CameraImage,
    
    [property: JsonPropertyName("source")]
    string? Source,
    
    [property: JsonPropertyName("results")]
    List<PythonAnalyzeResult> Results
    );