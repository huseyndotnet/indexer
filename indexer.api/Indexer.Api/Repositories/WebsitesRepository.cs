using Mapster;

using Ardalis.GuardClauses;

using Microsoft.EntityFrameworkCore;

using Indexer.Api.Data;
using Indexer.Api.Data.Entities;
using Indexer.Api.Repositories.Abstractions;
using Indexer.Api.DTOs;


namespace Indexer.Api.Repositories;

public class WebsitesRepository(IndexerDbContext dbContext) : IWebsitesRepository
{
    private readonly IndexerDbContext _dbContext = Guard.Against.Null(dbContext);


    public async Task<WebsiteDto?> GetWebsiteByDomain(string domain)
    {
        var response = await _dbContext.Websites
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Domain == domain);

        return response?.Adapt<WebsiteDto>();
    }

    public async Task SaveWebsite(WebsiteDto website)
    {
        var model = website.Adapt<Website>();

        await _dbContext.Websites.AddAsync(model);
        await _dbContext.SaveChangesAsync();
    }
}