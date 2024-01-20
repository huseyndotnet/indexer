using System.Net;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Indexer.Api.Data.Entities;

[Table("Website", Schema = IndexerDbContext.Schema)]
public class Website
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Domain { get; set; }

    public Address Address { get; set; }
}