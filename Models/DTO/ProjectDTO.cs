using System;

namespace project1.Models.DTO;

public class ProjectDTO
{
    public Guid Id { get; set; }
    public string ProjectName { get; set; }
    public string? ProjectDescriptionShort { get; set; }
    public string? ProjectDescriptionLong { get; set; }
    public string? ProjectLink { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModified { get; set; }
}
