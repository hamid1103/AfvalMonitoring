namespace AfvalMonitoring.Services
{
    using AfvalMonitoring.Models;
    using System.Text.Json;

    public class AfvalService
    {
        private readonly HttpClient _monitoringHttp;
        private readonly HttpClient _predictionHttp;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AfvalService(IHttpClientFactory httpClientFactory)
        {
            _monitoringHttp = httpClientFactory.CreateClient("MonitoringAPI");
            _predictionHttp = httpClientFactory.CreateClient("PredictionAPI");
        }

        private async Task<List<Detection>> GetDetections()
        {
            var json = await _monitoringHttp.GetStringAsync(_monitoringHttp.BaseAddress);
            return JsonSerializer.Deserialize<List<Detection>>(json, JsonOptions) ?? new List<Detection>();
        }

        public async Task<List<LabelCount>> GetLabelStats(int minConfidencePercentage = 0)
        {
            var threshold = Math.Clamp(minConfidencePercentage, 0, 100) / 100.0;
            var detections = await GetDetections();

            return detections
                .Where(d => !string.IsNullOrWhiteSpace(d.Label) && d.Confidence >= threshold)
                .GroupBy(d => d.Label)
                .Select(g => new LabelCount
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();
        }

        public async Task<List<AfvalPerDag>> GetDailyLabelStats(int minConfidencePercentage = 0)
        {
            var threshold = Math.Clamp(minConfidencePercentage, 0, 100) / 100.0;
            var detections = await GetDetections();

            return detections
                .Where(d => !string.IsNullOrWhiteSpace(d.Label) && d.Confidence >= threshold)
                .GroupBy(d => new { Date = d.Timestamp.Date, d.Label })
                .Select(g => new AfvalPerDag
                {
                    Date = g.Key.Date,
                    Label = g.Key.Label,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Label)
                .ToList();
        }

        public async Task<List<TrashLocation>> GetTrashLocations(int minConfidencePercentage = 0)
        {
            var threshold = Math.Clamp(minConfidencePercentage, 0, 100) / 100.0;
            var detections = await GetDetections();

            return detections
                .Where(d => !string.IsNullOrWhiteSpace(d.Label)
                    && d.Confidence >= threshold
                    && d.LocatieX.HasValue && d.LocatieY.HasValue)
                .Select(d => new TrashLocation
                {
                    Label = d.Label,
                    Confidence = (float)d.Confidence,
                    Latitude = d.LocatieX,
                    Longitude = d.LocatieY,
                    Adres = d.Location,
                    Tijd = d.Timestamp
                })
                .ToList();
        }

        public async Task<List<string>> GetAddresses()
        {
            var detections = await GetDetections();

            return detections
                .Select(d => d.Location)
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
            var json = await _predictionHttp.GetStringAsync(query);
            return JsonSerializer.Deserialize<PredictionResult>(json, JsonOptions);
        }
    }
}
