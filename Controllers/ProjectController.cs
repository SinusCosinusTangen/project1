using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Models.DTO;
using project1.Services;

namespace project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<ProjectListResponse>> GetProjects()
        {
            ProjectListResponse response = new ProjectListResponse();
            try
            {
                response.Response = 200;
                response.Message = "SUCCESS";
                response.Data = await _projectService.GetProjects();
            } 
            catch (Exception e)
            {
                return BadRequest();
            }

            return Ok(response);
        }

        // GET: api/Project/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> GetProject(Guid id)
        {
            ProjectResponse response = new ProjectResponse();
            try
            {
                var project = await _projectService.GetProject(id);

                if (project == null)
                {
                    return NotFound();
                }

                response.Response = 200;
                response.Message = "SUCCESS";
                response.Data = project;
            } 
            catch (Exception e)
            {   
                return BadRequest(response);
            }

            return Ok(response);
        }

        // PUT: api/Project/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectResponse>> PutProject(Guid id, ProjectDTO project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            try
            {
                return Ok(await _projectService.UpdateProject(id, project));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_projectService.ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Project
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> PostProject(ProjectDTO project)
        {
            return Ok(await _projectService.CreateProject(project));
        }

        // DELETE: api/Project/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _projectService.GetProject(id);
            if (!_projectService.ProjectExists(id))
            {
                return NotFound();
            }

            await _projectService.DeleteProject(id);

            return NoContent();
        }
    }
}
