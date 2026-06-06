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

        public async Task<List<AfvalPerDag>> GetDailyLabelStats(int minConfidencePercentage = 0)
        {
            var json = await _http.GetStringAsync("analytics/per-day");
            Console.WriteLine($"API RAW: {json}");

            using var document = JsonDocument.Parse(json);

            var threshold = Math.Clamp(minConfidencePercentage, 0, 100) / 100.0;

            var result = document.RootElement
                .EnumerateArray()
                .Select(item => new
                {
                    Dag = item.TryGetProperty("dag", out var dagProp) && DateTime.TryParse(dagProp.GetString(), out var dt)
                        ? dt
                        : DateTime.MinValue,
                    Label = item.TryGetProperty("label", out var labelProp)
                        ? labelProp.GetString()
                        : null,
                    Count = item.TryGetProperty("count", out var countProp) && countProp.TryGetInt32(out var count)
                        ? count
                        : 0
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

            Console.WriteLine($"API PARSED COUNT: {result.Count}");
            return result;
        }

        public async Task<List<TrashLocation>> GetTrashLocations(int minConfidencePercentage = 0)
        {
            var threshold = Math.Clamp(minConfidencePercentage, 0, 100) / 100.0;

            var json = await _http.GetStringAsync("afval");
            Console.WriteLine($"API RAW FOR MAP: {json}");

            using var document = JsonDocument.Parse(json);

            var result = document.RootElement
                .EnumerateArray()
                .Select(item => new
                {
                    Id = item.TryGetProperty("GUID", out var idProp) && Guid.TryParse(idProp.GetString(), out var id)
                        ? id
                        : (item.TryGetProperty("Id", out var idProp2) && Guid.TryParse(idProp2.GetString(), out var id2) ? id2 : (Guid?)null),
                    Label = item.TryGetProperty("Label", out var labelProp)
                        ? labelProp.GetString()
                        : (item.TryGetProperty("label", out var labelPropLower) ? labelPropLower.GetString() : null),
                    Confidence = item.TryGetProperty("Confidence", out var confidenceProp)
                        ? (float?)confidenceProp.GetDouble()
                        : (item.TryGetProperty("confidence", out var confidencePropLower) ? (float?)confidencePropLower.GetDouble() : null),
                    Latitude = item.TryGetProperty("Latitude", out var latProp) && latProp.ValueKind != JsonValueKind.Null
                        ? latProp.GetDouble()
                        : (item.TryGetProperty("latitude", out var latPropLower) && latPropLower.ValueKind != JsonValueKind.Null ? latPropLower.GetDouble() : GetCoordinatesFromAddress(item).lat),
                    Longitude = item.TryGetProperty("Longitude", out var lonProp) && lonProp.ValueKind != JsonValueKind.Null
                        ? lonProp.GetDouble()
                        : (item.TryGetProperty("longitude", out var lonPropLower) && lonPropLower.ValueKind != JsonValueKind.Null ? lonPropLower.GetDouble() : GetCoordinatesFromAddress(item).lng),
                    Locatie = item.TryGetProperty("Locatie", out var locatieProp)
                        ? locatieProp.GetString()
                        : (item.TryGetProperty("locatie", out var locatiePropLower) ? locatiePropLower.GetString() : null),
                    Adres = item.TryGetProperty("Adres", out var adresProp)
                        ? adresProp.GetString()
                        : (item.TryGetProperty("adres", out var adresPropLower) ? adresPropLower.GetString() : null),
                    Tijd = item.TryGetProperty("Tijd", out var tijdProp) && DateTime.TryParse(tijdProp.GetString(), out var dt)
                        ? dt
                        : (item.TryGetProperty("tijd", out var tijdPropLower) && DateTime.TryParse(tijdPropLower.GetString(), out var dtLower) ? dtLower : (DateTime?)null)
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Label)
                    && x.Confidence.HasValue && x.Confidence.Value >= threshold
                    && x.Latitude.HasValue && x.Longitude.HasValue)
                .Select(x => new TrashLocation
                {
                    Id = x.Id,
                    Label = x.Label,
                    Confidence = x.Confidence,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Locatie = x.Locatie,
                    Adres = x.Adres,
                    Tijd = x.Tijd
                })
                .ToList();

            Console.WriteLine($"MAP LOCATIONS COUNT: {result.Count}");
            if (result.Count > 0)
            {
                var firstWithCoords = result.FirstOrDefault(x => x.Latitude.HasValue && x.Longitude.HasValue);
                Console.WriteLine($"Locations with coordinates: {result.Count(x => x.Latitude.HasValue && x.Longitude.HasValue)}");
                Console.WriteLine($"First location: Label={result[0].Label}, Lat={result[0].Latitude}, Lng={result[0].Longitude}, Address={result[0].Locatie}");
            }
            return result;
        }

        private (double? lat, double? lng) GetCoordinatesFromAddress(JsonElement item)
        {
            if (item.TryGetProperty("Locatie", out var locatieProp))
            {
                var locatie = locatieProp.GetString();
                if (!string.IsNullOrWhiteSpace(locatie))
                {
                    var parts = locatie.Split(',');
                    if (parts.Length == 2
                        && double.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsedLat)
                        && double.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsedLng))
                    {
                        return (parsedLat, parsedLng);
                    }
                }
            }

            return (null, null);
        }
    }
}