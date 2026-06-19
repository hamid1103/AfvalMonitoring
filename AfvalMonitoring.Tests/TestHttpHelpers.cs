using System.Net;
using System.Text;

namespace AfvalMonitoring.Tests;

/// <summary>
/// Een eenvoudige HttpMessageHandler die altijd een vaste JSON-response teruggeeft,
/// zodat AfvalService getest kan worden zonder echte API-aanroepen.
/// </summary>
public class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseBody;

    public StubHttpMessageHandler(string responseBody)
    {
        _responseBody = responseBody;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_responseBody, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}

/// <summary>
/// Geeft voor elke benoemde client dezelfde gestubde HttpClient terug.
/// </summary>
public class StubHttpClientFactory : IHttpClientFactory
{
    private readonly string _responseBody;

    public StubHttpClientFactory(string responseBody)
    {
        _responseBody = responseBody;
    }

    public HttpClient CreateClient(string name)
    {
        return new HttpClient(new StubHttpMessageHandler(_responseBody))
        {
            BaseAddress = new Uri("https://example.test/")
        };
    }
}
