namespace AfvalMonitoring.Models;
using System.Text.Json.Serialization;

public class TrashLocation
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("confidence")]
    public float? Confidence { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("locatie")]
    public string? Locatie { get; set; }

    [JsonPropertyName("tijd")]
    public DateTime? Tijd { get; set; }
}
