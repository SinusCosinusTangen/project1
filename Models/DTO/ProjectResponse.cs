using Microsoft.AspNetCore.Mvc;

namespace project1.Models.DTO;
public class ProjectResponse
{
    public int Response { get; set; }
    public string? Message { get; set; }
    public ProjectDTO? Data { get; set; }
}