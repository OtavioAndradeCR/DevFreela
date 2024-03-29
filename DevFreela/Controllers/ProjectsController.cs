﻿using DevFreela.API.Models;
using DevFreela.Application.Commands.CreateComment;
using DevFreela.Application.Commands.CreateProject;
using DevFreela.Application.Commands.Delete;
using DevFreela.Application.Commands.UpdateProject;
using DevFreela.Application.InputModels;
using DevFreela.Application.Queries.GetAllProjects;
using DevFreela.Application.Queries.GetProjectById;
using DevFreela.Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DevFreela.API.Controllers
{
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
		public async Task<IActionResult> Get(string query)
		{
			var getAllProjectsQuery = new GetAllProjectsQuery(query);

			var projects = await _mediator.Send(getAllProjectsQuery);

			return Ok(projects);
		}

		// api/projects/2
		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync(int id)
		{
			var getProjectById = new GetProjectByIdQuery(id);

			var project = await _mediator.Send(getProjectById);

			if (project == null)
			{
				return NotFound();
			}
			return Ok(project);
		}
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] CreateProjectCommand command)
		{
			if (command.Title.Length > 50)
			{
				return BadRequest();
			}

			var id = await _mediator.Send(command);

			return CreatedAtAction(nameof(GetByIdAsync), new { id = id }, command);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Put(int id, [FromBody] UpdateProjectCommand command)
		{
			if (command.Description.Length > 200)
			{
				return BadRequest();
			}

			await _mediator.Send(command);

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var command = new DeleteProjectCommand(id);

			await _mediator.Send(command);

			return NoContent();
		}

		[HttpPost("{id}/comments")]
		public async Task<IActionResult> PostComment(int id, [FromBody] CreateCommentCommand command)
		{
			await _mediator.Send(command);

			return NoContent();
		}

		[HttpPut("{id}/start")]
		public IActionResult Start(int id)
		{
			_projectService.Start(id);

			return NoContent();
		}

		[HttpPut("{id}/finish")]
		public IActionResult Finish(int id)
		{
			_projectService.Finish(id);

			return NoContent();
		}
	}
}
