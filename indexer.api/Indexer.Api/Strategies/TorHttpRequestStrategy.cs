using Ardalis.GuardClauses;

using Indexer.Api.Handlers;
using Indexer.Api.Strategies.Abstractions;


namespace Indexer.Api.Strategies;

public class TorHttpRequestStrategy : IHttpRequestStrategy
{
    private readonly TorProxyHandler _torProxyHandler;


    public TorHttpRequestStrategy(TorProxyHandler torProxyHandler)
    {
        _torProxyHandler = Guard.Against.Null(torProxyHandler);
    }


    public async Task<HttpResponseMessage> SendHttpRequestAsync(string ipAddress)
    {
        try
        {
            using (var client = new HttpClient(_torProxyHandler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                string url = ipAddress.StartsWith("http://") ? ipAddress : "http://" + ipAddress;
                HttpResponseMessage response = await client.GetAsync(url);
                return response;
            }
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode is null)
                return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable);

            return new HttpResponseMessage(ex.StatusCode.Value);
        }
    }
}