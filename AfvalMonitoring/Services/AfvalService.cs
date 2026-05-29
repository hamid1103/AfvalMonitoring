
namespace AfvalMonitoring.Services
{
    using AfvalMonitoring.Models;

    public class AfvalService
    {
        private readonly HttpClient _http;

        public AfvalService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<LabelCount>> GetLabelStats(int minConfidencePercentage = 0)
        {
            var confidencePercentage = Math.Clamp(minConfidencePercentage, 0, 100);
            var confidence = confidencePercentage / 100.0;
            var endpoint = $"analytics/by-label?Confidence={confidence.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            var json = await _http.GetStringAsync(endpoint);
            Console.WriteLine($"API RAW: {json}");
            var result = System.Text.Json.JsonSerializer.Deserialize<List<LabelCount>>(json);
            Console.WriteLine($"API PARSED COUNT: {result?.Count}");
            return result ?? new List<LabelCount>();
        }
    }
}
