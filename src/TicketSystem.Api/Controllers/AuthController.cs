using MediatR;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Auth.Commands.Register;
using TicketSystem.Application.Auth.Queries.Login;

namespace TicketSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        var id = await mediator.Send(request);
        return CreatedAtAction(nameof(Login), new { id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginQuery request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}