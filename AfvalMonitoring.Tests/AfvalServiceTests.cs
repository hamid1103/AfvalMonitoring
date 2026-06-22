using AfvalMonitoring.Services;
using Xunit;

namespace AfvalMonitoring.Tests;

public class AfvalServiceTests
{
    private const string SampleJson = """
    [
        { "id": 1, "label": "Plastic", "confidence": 0.9, "timestamp": "2026-06-01T10:00:00", "location": "Hoofdstraat 1", "locatieX": 52.1, "locatieY": 4.3 },
        { "id": 2, "label": "Plastic", "confidence": 0.2, "timestamp": "2026-06-01T11:00:00", "location": "Hoofdstraat 1", "locatieX": 52.1, "locatieY": 4.3 },
        { "id": 3, "label": "Paper",   "confidence": 0.8, "timestamp": "2026-06-02T09:00:00", "location": "Kerkplein 5",  "locatieX": null, "locatieY": null },
        { "id": 4, "label": "",        "confidence": 0.99, "timestamp": "2026-06-02T09:30:00", "location": "Kerkplein 5", "locatieX": 52.2, "locatieY": 4.4 }
    ]
    """;

    private static AfvalService CreateService(string json = SampleJson)
        => new(new StubHttpClientFactory(json));

    [Fact]
    public async Task GetLabelStats_GroepeertEnTeltLabels()
    {
        var service = CreateService();

        var stats = await service.GetLabelStats();

        // Detectie zonder label valt af. Plastic = 2, Paper = 1
        Assert.Equal(2, stats.Count);
        Assert.Equal("Plastic", stats[0].Label); // hoogste count eerst
        Assert.Equal(2, stats[0].Count);
        Assert.Equal("Paper", stats[1].Label);
        Assert.Equal(1, stats[1].Count);
    }

    [Fact]
    public async Task GetLabelStats_FiltertOpConfidenceDrempel()
    {
        var service = CreateService();

        // Drempel van 50% laat de Plastic-detectie met confidence 0.2 vallen
        var stats = await service.GetLabelStats(minConfidencePercentage: 50);

        Assert.Equal(2, stats.Count);
        var plastic = Assert.Single(stats, s => s.Label == "Plastic");
        Assert.Equal(1, plastic.Count);
    }

    [Fact]
    public async Task GetDailyLabelStats_GroepeertPerDagEnLabel()
    {
        var service = CreateService();

        var daily = await service.GetDailyLabelStats();

        // 1 juni: Plastic (2 stuks). 2 juni: Paper (1 stuk). Lege labels tellen niet mee
        Assert.Equal(2, daily.Count);
        Assert.Equal(new DateTime(2026, 6, 1), daily[0].Date);
        Assert.Equal("Plastic", daily[0].Label);
        Assert.Equal(2, daily[0].Count);
        Assert.Equal(new DateTime(2026, 6, 2), daily[1].Date);
        Assert.Equal("Paper", daily[1].Label);
    }

    [Fact]
    public async Task GetTrashLocations_NeemtAlleenDetectiesMetCoordinaten()
    {
        var service = CreateService();

        var locations = await service.GetTrashLocations();

        // Alleen de twee Plastic-detecties hebben een geldig label en coördinaten
        // Paper mist coördinaten, de lege-label-detectie mist een label
        Assert.Equal(2, locations.Count);
        Assert.All(locations, l => Assert.Equal("Plastic", l.Label));
        Assert.All(locations, l =>
        {
            Assert.NotNull(l.Latitude);
            Assert.NotNull(l.Longitude);
        });
    }

    [Fact]
    public async Task GetAddresses_GeeftUniekeAdressen()
    {
        var service = CreateService();

        var addresses = await service.GetAddresses();

        // Hoofdstraat 1 komt 2 keer voor maar moet 1 keer terugkomen
        Assert.Equal(2, addresses.Count);
        Assert.Contains("Hoofdstraat 1", addresses);
        Assert.Contains("Kerkplein 5", addresses);
    }

    [Fact]
    public async Task GetLabelStats_LegeDataGeeftLegeLijst()
    {
        var service = CreateService("[]");

        var stats = await service.GetLabelStats();

        Assert.Empty(stats);
    }
}
