namespace Indexer.Api.Services.Abstractions;

public interface IIndexerService
{
    Task StartAsync();

    Task Stop();

    Task<bool> Status();
}