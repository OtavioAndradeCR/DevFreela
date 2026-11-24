using DevFreela.API.Models;
using DevFreela.Application.InputModels;
using DevFreela.Application.Queries.GetUser;
using DevFreela.Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevFreela.API.Controllers
{
    [ApiController]
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var user = await _mediator.Send(new GetUserById(id));
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateUserInputModel inputModel)
        {
            var id = _userService.Create(inputModel);
            return CreatedAtAction(nameof(GetByIdAsync), new { id }, inputModel);
        }

        [HttpPut("{id:int}/login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            return StatusCode(StatusCodes.Status501NotImplemented,
                "Autenticação ainda não está implementada neste projeto.");
        }
    }
}