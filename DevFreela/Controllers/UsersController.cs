using DevFreela.API.Models;
using DevFreela.Application.InputModels;
using DevFreela.Application.Queries.GetUser;
using DevFreela.Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevFreela.API.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
		private readonly IUserService _userService;
		private readonly IMediator _mediator;
		public UsersController(IUserService userService, IMediator mediator)
		{
			_userService = userService;
			_mediator = mediator;
		}

		// api/users/1
		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync(int id)
		{
			var getUserById = new GetUserByQuery(id);

			var user = await _mediator.Send(getUserById);

			if (user == null)
			{
				return NotFound();
			}
			return Ok(user);
		}
		// api/users
		[HttpPost]
		public IActionResult Post([FromBody] CreateUserInputModel inputModel)
		{
			var id = _userService.Create(inputModel);

			return CreatedAtAction(nameof(GetByIdAsync), new { id = id }, inputModel);
		}

		[HttpPut("{id}/login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            return NoContent();
        }
    }
}
