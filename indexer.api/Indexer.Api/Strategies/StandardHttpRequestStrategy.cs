using System.Net;

using Indexer.Api.Strategies.Abstractions;


namespace Indexer.Api.Strategies;

public class StandardHttpRequestStrategy : IHttpRequestStrategy
{
    public async Task<HttpResponseMessage> SendHttpRequestAsync(string ipAddress)
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                string url = ipAddress.StartsWith("http://") ? ipAddress : "http://" + ipAddress;
                HttpResponseMessage response = await client.GetAsync(url);
                return response;
            }
        }
        catch (HttpRequestException ex) when (ex.InnerException is HttpRequestException innerException)
        {
            if (innerException.InnerException is WebException webException && webException.Status == WebExceptionStatus.NameResolutionFailure)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
        catch (HttpRequestException ex)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}