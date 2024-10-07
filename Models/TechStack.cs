using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project1.Models;

[Table("tech_stack")]
public class TechStack
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Column("project_id")]
    public Guid ProjectId { get; set;}
    [Column("name")]
    public string Name { get; set;}
    [Column("type")]
    public string Type { get; set;}
    [Column("created_date")]
    public DateTime CreatedDate { get; set;}
    [Column("last_modified")]
    public DateTime LastModified { get; set;}
}