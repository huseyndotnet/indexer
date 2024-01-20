using Microsoft.EntityFrameworkCore;


namespace Indexer.Api.Data;

public class IndexerDbContext : DbContext
{
    public const string Schema = "Indexer";

    public IndexerDbContext(DbContextOptions<IndexerDbContext> options) : base(options) { }


    public DbSet<Data.Entities.Website> Websites { get; set; }


    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
    }
}