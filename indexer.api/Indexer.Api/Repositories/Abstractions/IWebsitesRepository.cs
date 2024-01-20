using Indexer.Api.DTOs;


namespace Indexer.Api.Repositories.Abstractions;

public interface IWebsitesRepository
{
    Task<WebsiteDto?> GetWebsiteByDomain(string domain);

    Task SaveWebsite(WebsiteDto response);
}