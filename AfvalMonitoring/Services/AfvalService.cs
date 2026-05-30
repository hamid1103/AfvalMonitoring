
namespace AfvalMonitoring.Services
{
    using AfvalMonitoring.Models;
    using System.Net.Http.Json;

    public class AfvalService
    {
        private readonly HttpClient _http;

        public AfvalService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<LabelCount>> GetLabelStats()
        {
            var json = await _http.GetStringAsync("analytics/by-label");
            Console.WriteLine($"API RAW: {json}");
            var result = System.Text.Json.JsonSerializer.Deserialize<List<LabelCount>>(json);
            Console.WriteLine($"API PARSED COUNT: {result?.Count}");
            return result ?? new List<LabelCount>();
        }
    }
}
