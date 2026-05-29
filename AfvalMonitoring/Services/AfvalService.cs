
namespace AfvalMonitoring.Services
{
    using AfvalMonitoring.Models;
    using System.Text.Json;

    public class AfvalService
    {
        private readonly HttpClient _http;

        public AfvalService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<LabelCount>> GetLabelStats(int minConfidencePercentage = 0)
        {
            var threshold = Math.Clamp(minConfidencePercentage, 0, 100) / 100.0;

            var json = await _http.GetStringAsync("afval");
            Console.WriteLine($"API RAW: {json}");

            using var document = JsonDocument.Parse(json);

            var result = document.RootElement
                .EnumerateArray()
                .Select(item => new
                {
                    Label = item.TryGetProperty("Label", out var labelProp)
                        ? labelProp.GetString()
                        : (item.TryGetProperty("label", out var labelPropLower) ? labelPropLower.GetString() : null),
                    Confidence = item.TryGetProperty("Confidence", out var confidenceProp)
                        ? confidenceProp.GetDouble()
                        : (item.TryGetProperty("confidence", out var confidencePropLower) ? confidencePropLower.GetDouble() : 0d)
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Label) && x.Confidence >= threshold)
                .GroupBy(x => x.Label!)
                .Select(g => new LabelCount
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            Console.WriteLine($"API PARSED COUNT: {result.Count}");
            return result;
        }
    }
}
