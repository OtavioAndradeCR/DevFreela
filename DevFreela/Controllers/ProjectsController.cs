using DevFreela.Application.Commands.CreateComment;
using DevFreela.Application.Commands.CreateProject;
using DevFreela.Application.Commands.Delete;
using DevFreela.Application.Commands.UpdateProject;
using DevFreela.Application.Queries.GetAllProjects;
using DevFreela.Application.Queries.GetProjectById;
using DevFreela.Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevFreela.API.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMediator _mediator;

        public ProjectsController(IProjectService projectService, IMediator mediator)
        {
            _projectService = projectService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string query)
        {
            var projects = await _mediator.Send(new GetAllProjectsQuery(query));
            return Ok(projects);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var project = await _mediator.Send(new GetProjectByIdQuery(id));
            return project == null ? NotFound() : Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProjectCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByIdAsync), new { id }, command);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateProjectCommand command)
        {
            command.Id = id;
            var updated = await _mediator.Send(command);
            return updated ? NoContent() : NotFound();
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var cancelled = await _mediator.Send(new DeleteProjectCommand(id));
            return cancelled ? NoContent() : NotFound();
        }

        [HttpPost("{id:int}/comments")]
        public async Task<IActionResult> PostComment(int id, [FromBody] CreateCommentCommand command)
        {
            command.IdProject = id;
            var created = await _mediator.Send(command);
            return created ? NoContent() : NotFound();
        }

        [HttpPut("{id:int}/start")]
        public async Task<IActionResult> Start(int id, CancellationToken cancellationToken)
        {
            var started = await _projectService.Start(id, cancellationToken);
            return started ? NoContent() : NotFound();
        }

        [HttpPut("{id:int}/finish")]
        public async Task<IActionResult> Finish(int id, CancellationToken cancellationToken)
        {
            var finished = await _projectService.Finish(id, cancellationToken);
            return finished ? NoContent() : NotFound();
        }
    }
}