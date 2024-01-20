using System.Net;

using Ardalis.GuardClauses;

using Indexer.Api.Options;


namespace Indexer.Api.Handlers;

public class TorProxyHandler : HttpClientHandler
{
    private readonly TorProxyConfig _torConfig;


    public TorProxyHandler(TorProxyConfig torProxy)
    {
        _torConfig = Guard.Against.Null(torProxy);
    }


    protected sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using (var httpClient = new HttpClient(new HttpClientHandler
        {
            Proxy = new WebProxy(_torConfig.Proxy),
            UseProxy = _torConfig.UseProxy,
        }))
        {
            var clonedRequest = await Helpers.CloneHttpRequestHelper.CloneHttpRequestMessage(request);

            return await httpClient.SendAsync(clonedRequest, cancellationToken);
        }
    }
}