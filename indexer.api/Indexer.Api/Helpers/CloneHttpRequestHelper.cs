namespace Indexer.Api.Helpers;

public static class CloneHttpRequestHelper
{

    public static async Task<HttpRequestMessage> CloneHttpRequestMessage(HttpRequestMessage request)
    {
        var clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri);

        foreach (var (key, value) in request.Headers)
            clonedRequest.Headers.TryAddWithoutValidation(key, value);

        if (request.Content != null)
        {
            var content = await request.Content.ReadAsByteArrayAsync();
            clonedRequest.Content = new ByteArrayContent(content);

            if (request.Content.Headers.TryGetValues("Content-Type", out var contentTypes))
                clonedRequest.Content.Headers.TryAddWithoutValidation("Content-Type", contentTypes.ToArray());

        }

        return clonedRequest;
    }
}