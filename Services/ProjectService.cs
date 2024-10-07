using Microsoft.EntityFrameworkCore;
using project1.Models;
using project1.Models.DTO;

namespace project1.Services
{
	public class ProjectService
	{
		private ProjectDTO projectDTO;
		private ProjectContext _context;

		public ProjectService(ProjectContext projectContext)
		{
			this._context = projectContext;
		}

		public async Task<IEnumerable<ProjectDTO>> GetProjects()
		{
			var projects = await _context.projects.ToListAsync();
			var result = projects.Select(project => ProjectToProjectDTO(project)).ToList();

			return result;
		}

		public async Task<ProjectDTO> GetProject(Guid id)
		{
			var project = await _context.projects.FindAsync(id);

			if (project != null)
			{
				ProjectDTO projectDTO = ProjectToProjectDTO(project);

				return projectDTO;
			}

			return null;
		}

		public async Task<ProjectDTO> UpdateProject(Guid id, ProjectDTO projectDTO)
		{
			Project existingProject = await _context.projects.FindAsync(id);
			
			if (existingProject == null)
			{
				throw new Exception();
			}

			existingProject.ProjectName = projectDTO.ProjectName;
			existingProject.ProjectDescription = projectDTO.ProjectDescriptionLong;
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
			project.LastModified = DateTime.Now;
			
			_context.projects.Add(project);
			await _context.SaveChangesAsync();

			return ProjectToProjectDTO(project);
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
			project.ProjectDescription = projectDTO.ProjectDescriptionLong;
			project.CreatedDate = (DateTime)projectDTO.CreatedDate;
			project.LastModified = (DateTime)projectDTO.LastModified;

			return project;
		}

		private ProjectDTO ProjectToProjectDTO(Project project)
		{
			ProjectDTO projectDTO = new ProjectDTO();
			projectDTO.Id = project.Id;
			projectDTO.ProjectName = project.ProjectName;
			projectDTO.ProjectDescriptionShort = GetSubstring(project.ProjectDescription, 50);
			projectDTO.ProjectDescriptionLong = project.ProjectDescription;
			projectDTO.CreatedDate = project.CreatedDate;
			projectDTO.LastModified = project.LastModified;

			return projectDTO;
		}
	}
}