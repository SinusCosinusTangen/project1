using Microsoft.EntityFrameworkCore;
using project1.Models;

namespace project1.Services
{
	public class TechStackService(TechStackContext techStackContext)
    {
		private readonly TechStackContext _context = techStackContext;

        public async Task<IEnumerable<TechStack>> GetTechStacks(Guid projectId)
		{
			return await _context.techStacks.Where(tech => tech.ProjectId == projectId).ToListAsync();
		}

		public async Task<TechStack> AddTechStack(TechStack techStack)
		{
			techStack.CreatedDate = DateTime.Now;
            techStack.LastModified = DateTime.Now;

            var existingTechStack = await _context.techStacks
                .AsNoTracking()
                .FirstOrDefaultAsync(ts => ts.ProjectId == techStack.ProjectId && ts.Name == techStack.Name && ts.Type == techStack.Type);

            if (existingTechStack != null)
            {
                throw new InvalidOperationException("A tech stack with the same Id already exists");
            }

            await _context.techStacks.AddAsync(techStack);
			await _context.SaveChangesAsync();

			return techStack;
		}

		public bool TechStackExist(Guid projectId, string name, string type)
		{
			return _context.techStacks.Any(e => e.ProjectId == projectId && e.Name == name && e.Type == type);
		}
	}
}