using project1.Models.DTO;

namespace project1.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDTO>> GetProjects();
        Task<ProjectDTO> GetProject(Guid id);
        Task<ProjectDTO> UpdateProject(Guid id, ProjectDTO projectDTO);
        Task<ProjectDTO> CreateProject(ProjectDTO projectDTO);
        Task DeleteProject(Guid id);
    }
}
