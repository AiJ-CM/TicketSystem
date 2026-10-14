using MediatR;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Tickets.Commands.CreateTicket;

namespace TicketSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<long>> CreateTicket([FromBody] CreateTicketCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTicket), new { id }, id);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<int>> GetTicket(long id, CancellationToken cancellationToken)
    {
        // 下一步补
        return Ok();
    }
}