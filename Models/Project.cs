using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project1.Models;

public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set;}
    [Column("project_name")]
    public string ProjectName { get; set;}
    [Column("project_description")]
    public string ProjectDescription { get; set;}
    [Column("project_link")]
    public string ProjectLink { get; set;}
    [Column("created_date")]
    public DateTime CreatedDate { get; set;}
    [Column("last_modified")]
    public DateTime LastModified { get; set;}
}