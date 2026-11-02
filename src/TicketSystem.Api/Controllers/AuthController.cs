using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Auth.Commands.Register;
using TicketSystem.Application.Auth.Queries.GetUser;
using TicketSystem.Application.Auth.Queries.Login;

namespace TicketSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(
            actionName: nameof(GetProfile),
            routeValues: null, 
            value: new { Id = id }
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginQuery request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUserProfileQuery(), cancellationToken);

        return Ok(user);
    }
}