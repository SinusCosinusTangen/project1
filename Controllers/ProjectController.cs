using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Models.DTO;
using project1.Services;
using project1.Constant;
using project1.Exceptions;

namespace project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController(ProjectService projectService) : ControllerBase
    {
        private readonly ProjectService _projectService = projectService;

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<ProjectDTO>>>> GetProjects()
        {
            try
            {
                return ConvertToResponse(ApiConstant.OK, ApiConstant.OK_MESSAGE, await _projectService.GetProjects());
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

                return Ok(ConvertToResponse(ApiConstant.OK, ApiConstant.OK_MESSAGE, project));
            } 
            catch (NotFoundException)
            {  
                return NotFound(ConvertToResponse(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "Project not found"));
            }
        }

        // PUT: api/Project/xxx
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<ProjectDTO>>> PutProject(Guid id, ProjectDTO project)
        {
            if (id != project.Id)
            {
                return BadRequest(ConvertToResponse(ApiConstant.BAD_REQUEST, ApiConstant.BAD_REQUEST_MESSAGE, "ID not match"));
            }

            try
            {
                var updatedProject = await _projectService.UpdateProject(id, project);

                return Ok(ConvertToResponse(ApiConstant.OK, ApiConstant.OK_MESSAGE, updatedProject));
            }
            catch (NotFoundException)
            {
                return NotFound(ConvertToResponse(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "Project not found"));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        // POST: api/Project
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> PostProject(ProjectDTO project)
        {
            var createdProject = await _projectService.CreateProject(project);
            return Ok(ConvertToResponse(ApiConstant.OK, ApiConstant.OK_MESSAGE, createdProject));
        }

        // DELETE: api/Project/xxx
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                await _projectService.DeleteProject(id);

                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound(ConvertToResponse(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "Project not found"));
            }
        }

        private static Response<ProjectDTO> ConvertToResponse(int code, string message, ProjectDTO projectDTO)
        {
            return new Response<ProjectDTO>() {
                Code = code,
                Message = message,
                Data = projectDTO
            };
        }

        private static Response<IEnumerable<ProjectDTO>> ConvertToResponse(int code, string message, IEnumerable<ProjectDTO> projectDTO)
        {
            return new Response<IEnumerable<ProjectDTO>>() {
                Code = code,
                Message = message,
                Data = projectDTO
            };
        }

        private static Response<string> ConvertToResponse(int code, string message, string data)
        {
            return new Response<string>() {
                Code = code,
                Message = message,
                Data = data
            };
        }
    }
}
