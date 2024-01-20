using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Indexer.Api.Data.Entities;

[Table("Address", Schema = IndexerDbContext.Schema)]
public class Address
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string IpAddress { get; set; }

    public string Domain { get; set; }

    public int StatusCode { get; set; }

    public int ResponseTimeMs { get; set; }

    public DateTime Timestamp { get; set; }

    [ForeignKey(nameof(Website))]
    public int? WebsiteId { get; set; }

    public Website Website { get; set; }

    [ForeignKey(nameof(Subdomain))]
    public int? SubdomainId { get; set; }

    public Subdomain Subdomain { get; set; }
}