namespace AfvalMonitoring.Models;
using System.Text.Json.Serialization;

public class Detection
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("locatieX")]
    public double? LocatieX { get; set; }

    [JsonPropertyName("locatieY")]
    public double? LocatieY { get; set; }
}
