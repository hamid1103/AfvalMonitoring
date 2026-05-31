namespace AfvalMonitoring.Models;
using System.Text.Json.Serialization;

public class AfvalPerDag
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }
}
