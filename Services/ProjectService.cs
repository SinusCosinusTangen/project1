using Microsoft.EntityFrameworkCore;
using project1.Models;
using project1.Models.DTO;

namespace project1.Services
{
	public class ProjectService
	{
		private ProjectContext _context;
		private readonly TechStackService _techStackService;

		public ProjectService(ProjectContext projectContext, TechStackService techStackService)
		{
			this._context = projectContext;
			this._techStackService = techStackService;
		}

		public async Task<IEnumerable<ProjectDTO>> GetProjects()
		{
			var projects = await _context.projects.ToListAsync();
			// Create a list of tasks for the conversion
			var projectDTOTasks = projects.Select(project => ProjectToProjectDTO(project));

			// Await all tasks and return the result
			var projectDTOs = await Task.WhenAll(projectDTOTasks);

			return projectDTOs.ToList(); // Convert the array to a list if needed
		}

		public async Task<ProjectDTO> GetProject(Guid id)
		{
			var project = await _context.projects.FindAsync(id);

			if (project != null)
			{
				ProjectDTO projectDTO = await ProjectToProjectDTO(project);

				return projectDTO;
			}

			return null;
		}

		public async Task<ProjectDTO> UpdateProject(Guid id, ProjectDTO projectDTO)
		{
			Project existingProject = await _context.projects.FindAsync(id) ?? throw new Exception();
            existingProject.ProjectName = projectDTO.ProjectName is null? existingProject.ProjectName : projectDTO.ProjectName;
			existingProject.ProjectDescription = projectDTO.ProjectDescriptionLong is null? existingProject.ProjectDescription : projectDTO.ProjectDescriptionLong;
			existingProject.ProjectLink = projectDTO.ProjectLink is null? existingProject.ProjectLink : projectDTO.ProjectLink;
			existingProject.LastModified = DateTime.Now;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				throw;
			}

			return await GetProject(id);
		}

		public async Task<ProjectDTO> CreateProject(ProjectDTO projectDTO)
		{
			Project project = ProjectDTOToProject(projectDTO);
			project.CreatedDate = DateTime.Now;
			
			_context.projects.Add(project);
			await _context.SaveChangesAsync();

			List<TechStack> techStacks = projectDTO.techStacks;
			foreach (TechStack techStack in techStacks)
			{
				techStack.ProjectId = project.Id;
				await _techStackService.AddTechStack(techStack);
			}

			return await ProjectToProjectDTO(project);
		}

		public async Task DeleteProject(Guid id)
		{
			Project project = await _context.projects.FindAsync(id);
			_context.projects.Remove(project);
			await _context.SaveChangesAsync();
		}
		

		public bool ProjectExists(Guid id)
		{
			return _context.projects.Any(e => e.Id == id);
		}

		private string GetSubstring(string input, int n)
		{
			if (string.IsNullOrEmpty(input) || n <= 0)
			{
				return string.Empty;
			}

			return input.Substring(0, Math.Min(n, input.Length));
		}

		private Project ProjectDTOToProject(ProjectDTO projectDTO)
		{
			Project project = new Project();
			project.Id = projectDTO.Id;
			project.ProjectName = projectDTO.ProjectName;
			project.ProjectDescription = projectDTO.ProjectDescriptionLong is null? "" : projectDTO.ProjectDescriptionLong;
			project.ProjectLink = projectDTO.ProjectLink is null? "" : projectDTO.ProjectLink;
			project.CreatedDate = projectDTO.CreatedDate is null? (DateTime)DateTime.Now : (DateTime)projectDTO.CreatedDate;
			project.CreatedDate = projectDTO.LastModified is null? (DateTime)DateTime.Now : (DateTime)projectDTO.LastModified;

			return project;
		}

		private async Task<ProjectDTO> ProjectToProjectDTO(Project project)
		{
			ProjectDTO projectDTO = new ProjectDTO();
			projectDTO.Id = project.Id;
			projectDTO.ProjectName = project.ProjectName;
			projectDTO.ProjectDescriptionShort = project.ProjectDescription.Length > 100 ? string.Concat(GetSubstring(project.ProjectDescription, 100), "...") : project.ProjectDescription;
			projectDTO.ProjectDescriptionLong = project.ProjectDescription;
			projectDTO.ProjectLink = project.ProjectLink;
			projectDTO.techStacks = (List<TechStack>)await _techStackService.GetTechStacks(project.Id);
			projectDTO.CreatedDate = project.CreatedDate;
			projectDTO.LastModified = project.LastModified;

			return projectDTO;
		}
	}
}