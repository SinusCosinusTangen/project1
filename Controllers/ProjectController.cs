using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Models.DTO;
using project1.Services;
using project1.Constant;
using project1.Exceptions;
using project1.Helpers;

namespace project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController(IProjectService projectService) : ControllerBase
    {
        private readonly IProjectService _projectService = projectService;

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<ProjectDTO>>>> GetProjects()
        {
            try
            {
                return Ok(new Response<IEnumerable<ProjectDTO>>(ApiConstant.OK, ApiConstant.OK_MESSAGE, await _projectService.GetProjects()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: api/Project/xxx
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<ProjectDTO>>> GetProject(Guid id)
        {
            try
            {
                var project = await _projectService.GetProject(id);

                return Ok(new Response<ProjectDTO>(ApiConstant.OK, ApiConstant.OK_MESSAGE, project));
            }
            catch (NotFoundException)
            {
                return NotFound(new Response<string>(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "PROJECT NOT FOUND"));
            }
        }

        // PUT: api/Project/xxx
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Response<ProjectDTO>>> PutProject(Guid id, ProjectDTO project)
        {
            if (id != project.Id)
            {
                return BadRequest(new Response<string>(ApiConstant.BAD_REQUEST, ApiConstant.BAD_REQUEST_MESSAGE, "ID NOT MATCH"));
            }

            try
            {
                var updatedProject = await _projectService.UpdateProject(id, project);

                return Ok(new Response<ProjectDTO>(ApiConstant.OK, ApiConstant.OK_MESSAGE, updatedProject));
            }
            catch (NotFoundException)
            {
                return NotFound(new Response<string>(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "PROJECT NOT FOUND"));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        // POST: api/Project
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProjectDTO>> PostProject(ProjectDTO project)
        {
            try
            {
                var createdProject = await _projectService.CreateProject(project);
                return Ok(new Response<ProjectDTO>(ApiConstant.OK, ApiConstant.OK_MESSAGE, createdProject));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // DELETE: api/Project/xxx
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                await _projectService.DeleteProject(id);

                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound(new Response<string>(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "Project not found"));
            }
        }
    }
}
