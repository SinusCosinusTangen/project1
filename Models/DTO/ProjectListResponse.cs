using Microsoft.AspNetCore.Mvc;

namespace project1.Models.DTO;
public class ProjectListResponse
{
    public int Response { get; set; }
    public string? Message { get; set; }
    public IEnumerable<ProjectDTO>? Data { get; set; }
}