using Microsoft.EntityFrameworkCore;
using project1.Exceptions;
using project1.Models;
using project1.Models.DTO;

namespace project1.Services
{
	public class ProjectService : IProjectService
	{
		private readonly ProjectContext _context;
		private readonly ITechStackService _techStackService;

		// Correct constructor syntax
		public ProjectService(ProjectContext projectContext, ITechStackService techStackService)
		{
			_context = projectContext;
			_techStackService = techStackService;
		}

		public async Task<IEnumerable<ProjectDTO>> GetProjects()
		{
			var projects = await _context.projects.ToListAsync();

			var projectDTOs = new List<ProjectDTO>();
			foreach (var project in projects)
			{
				var projectDTO = await ProjectToProjectDTO(project);
				projectDTOs.Add(projectDTO);
			}

			return projectDTOs;
		}

		public async Task<ProjectDTO> GetProject(Guid id)
		{
			var project = await _context.projects.FindAsync(id) ?? throw new NotFoundException();
			ProjectDTO projectDTO = await ProjectToProjectDTO(project);

			return projectDTO;
		}

		public async Task<ProjectDTO> UpdateProject(Guid id, ProjectDTO projectDTO)
		{
			Project existingProject = await _context.projects.FindAsync(id) ?? throw new NotFoundException();
			existingProject.ProjectName = projectDTO.ProjectName is null ? existingProject.ProjectName : projectDTO.ProjectName;
			existingProject.ProjectDescription = projectDTO.ProjectDescriptionLong is null ? existingProject.ProjectDescription : projectDTO.ProjectDescriptionLong;
			existingProject.ProjectLink = projectDTO.ProjectLink is null ? existingProject.ProjectLink : projectDTO.ProjectLink;
			existingProject.LastModified = DateTime.Now;

			IEnumerable<TechStack> existingTechStacks = await _techStackService.GetTechStacks(existingProject.Id);

			var techStacksToRemove = existingTechStacks
				.Where(existing => !projectDTO.techStacks.Any(dto => dto.Name.ToLower() == existing.Name.ToLower()))
				.ToList();

			foreach (TechStack techStack in techStacksToRemove)
			{
				Console.WriteLine(techStack.Name);
				Console.WriteLine(techStack.Id);
				try
				{
					await _techStackService.RemoveTechStack(techStack);
				}
				catch (InvalidOperationException ex)
				{
					continue;
				}
			}

			foreach (TechStack techStack in projectDTO.techStacks)
			{
				try
				{
					if (!existingTechStacks.Any(existing => existing.Name.ToLower() == techStack.Name.ToLower()))
					{
						techStack.ProjectId = existingProject.Id;
						await _techStackService.AddTechStack(techStack);
					}
				}
				catch (InvalidOperationException)
				{
					continue;
				}
			}

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
			Project project = await _context.projects.FindAsync(id) ?? throw new NotFoundException();
			await _techStackService.RemoveTechStacks(project.Id);
			_context.projects.Remove(project);
			await _context.SaveChangesAsync();
		}

		private static string GetSubstring(string input, int n)
		{
			if (string.IsNullOrEmpty(input) || n <= 0)
			{
				return string.Empty;
			}

			return input[..Math.Min(n, input.Length)];
		}

		private static Project ProjectDTOToProject(ProjectDTO projectDTO)
		{
			Project project = new()
			{
				Id = projectDTO.Id,
				ProjectName = projectDTO.ProjectName,
				ProjectDescription = projectDTO.ProjectDescriptionLong is null ? "" : projectDTO.ProjectDescriptionLong,
				ProjectLink = projectDTO.ProjectLink is null ? "" : projectDTO.ProjectLink,
				CreatedDate = projectDTO.CreatedDate is null ? DateTime.Now : (DateTime)projectDTO.CreatedDate,
				LastModified = projectDTO.LastModified is null ? DateTime.Now : (DateTime)projectDTO.LastModified
			};

			return project;
		}

		private async Task<ProjectDTO> ProjectToProjectDTO(Project project)
		{
			ProjectDTO projectDTO = new()
			{
				Id = project.Id,
				ProjectName = project.ProjectName,
				ProjectDescriptionShort = project.ProjectDescription.Length > 100 ? string.Concat(GetSubstring(project.ProjectDescription, 100), "...") : project.ProjectDescription,
				ProjectDescriptionLong = project.ProjectDescription,
				ProjectLink = project.ProjectLink,
				CreatedDate = project.CreatedDate,
				LastModified = project.LastModified,
				techStacks = (await _techStackService.GetTechStacks(project.Id)).ToList()
			};

			return projectDTO;
		}
	}
}