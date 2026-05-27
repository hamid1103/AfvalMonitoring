namespace AfvalMonitoring.Models;
using System.Text.Json.Serialization;

public class LabelCount
{
    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }
}
