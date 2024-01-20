using Ardalis.GuardClauses;

using Indexer.Api.Strategies.Abstractions;


namespace Indexer.Api.Helpers;

public class HttpRequestHelper
{
    private readonly IHttpRequestStrategy _httpRequestStrategy;


    public HttpRequestHelper(IHttpRequestStrategy httpRequestStrategy)
    {
        _httpRequestStrategy = Guard.Against.Null(httpRequestStrategy);
    }


    public async Task<HttpResponseMessage> SendHttpRequestAsync(string ipAddress)
    {
        return await _httpRequestStrategy.SendHttpRequestAsync(ipAddress);
    }
}