namespace Indexer.Api.Strategies.Abstractions;

public interface IHttpRequestStrategy
{
    Task<HttpResponseMessage> SendHttpRequestAsync(string ipAddress);
}