using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Api.Extensions;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Tickets.Commands.AssignTicket;
using TicketSystem.Application.Tickets.Commands.CreateTicket;
using TicketSystem.Application.Tickets.Commands.UpdateStatus;
using TicketSystem.Application.Tickets.Dtos;
using TicketSystem.Application.Tickets.Queries.GetTicket;
using TicketSystem.Application.Tickets.Queries.GetTicketList;

namespace TicketSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<long>> CreateTicket([FromBody] CreateTicketCommand command, CancellationToken cancellationToken)
    {
        command = command with { CreatorId = User.GetUserId() };
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTicket), new { id }, id);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TicketDto>> GetTicket(long id, CancellationToken cancellationToken)
    {
        var query = new GetTicketQuery(id);
        var ticketDto = await mediator.Send(query, cancellationToken);
        return Ok(ticketDto);
    }

    [HttpPost("assign")]
    [Authorize]
    public async Task<IActionResult> Assign(AssignTicketCommand command, CancellationToken cancellationToken)
    {
        command = command with { CurrentUserId = User.GetUserId() };
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("updateStatus")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(UpdateTicketStatusCommand command, CancellationToken cancellationToken)
    {
        command = command with { CurrentUserId = User.GetUserId() };
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedList<TicketDto>>> GetTickets(
    [FromQuery] GetTicketListQuery query,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}