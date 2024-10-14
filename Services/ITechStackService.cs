using project1.Models;

namespace project1.Services
{
    public interface ITechStackService
    {
        Task<IEnumerable<TechStack>> GetTechStacks(Guid projectId);
        Task<TechStack> AddTechStack(TechStack techStack);
        Task<int> RemoveTechStacks(Guid projectId);
        Task<int> RemoveTechStack(TechStack techStack);
        bool TechStackExist(Guid projectId, string name, string type);
    }
}
