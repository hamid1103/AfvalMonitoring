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

            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .EnumerateArray()
                .Select(item => new
                {
                    Label = item.TryGetProperty("label", out var labelProp) ? labelProp.GetString() : null,
                    Confidence = item.TryGetProperty("confidence", out var confidenceProp) ? confidenceProp.GetDouble() : 0d
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Label) && x.Confidence >= threshold)
                .GroupBy(x => x.Label!)
                .Select(g => new LabelCount { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();
        }

        public async Task<List<AfvalPerDag>> GetDailyLabelStats(int minConfidencePercentage = 0)
        {
            var json = await _http.GetStringAsync("analytics/per-day");

            using var document = JsonDocument.Parse(json);

            var threshold = Math.Clamp(minConfidencePercentage, 0, 100) / 100.0;

            return document.RootElement
                .EnumerateArray()
                .Select(item => new
                {
                    Dag = item.TryGetProperty("dag", out var dagProp) && DateTime.TryParse(dagProp.GetString(), out var dt) ? dt : DateTime.MinValue,
                    Label = item.TryGetProperty("label", out var labelProp) ? labelProp.GetString() : null,
                    Count = item.TryGetProperty("count", out var countProp) && countProp.TryGetInt32(out var count) ? count : 0
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Label))
                .Select(x => new AfvalPerDag
                {
                    Date = x.Dag.Date,
                    Label = x.Label,
                    Count = x.Count
                })
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Label)
                .ToList();
        }

        public async Task<List<TrashLocation>> GetTrashLocations(int minConfidencePercentage = 0)
        {
            var threshold = Math.Clamp(minConfidencePercentage, 0, 100) / 100.0;

            var json = await _http.GetStringAsync("afval");

            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .EnumerateArray()
                .Select(item => new
                {
                    Id = item.TryGetProperty("id", out var idProp) && idProp.TryGetInt32(out var id) ? (int?)id : null,
                    Label = item.TryGetProperty("label", out var labelProp) ? labelProp.GetString() : null,
                    Confidence = item.TryGetProperty("confidence", out var confidenceProp) ? (double?)confidenceProp.GetDouble() : null,
                    Latitude = item.TryGetProperty("locatieX", out var latProp) && latProp.ValueKind != JsonValueKind.Null ? (double?)latProp.GetDouble() : null,
                    Longitude = item.TryGetProperty("locatieY", out var lonProp) && lonProp.ValueKind != JsonValueKind.Null ? (double?)lonProp.GetDouble() : null,
                    Location = item.TryGetProperty("location", out var locationProp) ? locationProp.GetString() : null,
                    Tijd = item.TryGetProperty("timestamp", out var tijdProp) && DateTime.TryParse(tijdProp.GetString(), out var dt) ? (DateTime?)dt : null
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Label)
                    && x.Confidence.HasValue && x.Confidence.Value >= threshold
                    && x.Latitude.HasValue && x.Longitude.HasValue)
                .Select(x => new TrashLocation
                {
                    Id = x.Id,
                    Label = x.Label!,
                    Confidence = (float?)x.Confidence,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Locatie = x.Location,
                    Adres = x.Location,
                    Tijd = x.Tijd
                })
                .ToList();
        }

        public async Task<List<string>> GetAddresses()
        {
            var json = await _http.GetStringAsync("afval");

            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .EnumerateArray()
                .Select(item => item.TryGetProperty("location", out var locationProp) ? locationProp.GetString() : null)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a!)
                .Distinct()
                .ToList();
        }

        public async Task<PredictionResult?> GetPrediction(string label, string street, DateTime startDate, DateTime endDate)
        {
            var query = $"prediction/simple?label={Uri.EscapeDataString(label)}" +
                        $"&street={Uri.EscapeDataString(street)}" +
                        $"&start={startDate:yyyy-MM-dd}&end={endDate:yyyy-MM-dd}";
            var json = await _http.GetStringAsync(query);
            return JsonSerializer.Deserialize<PredictionResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
