using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Indexer.Api.Data.Entities;

[Table("Subdomain", Schema = IndexerDbContext.Schema)]
public class Subdomain
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [ForeignKey(nameof(Website))]
    public int WebsiteId { get; set; }

    public Website Website { get; set; }
}