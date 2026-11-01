using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Api.Extensions;
using TicketSystem.Application.Audit.Queries.GetTicketAudits;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Tickets.Commands.AssignTicket;
using TicketSystem.Application.Tickets.Commands.CloseTicket;
using TicketSystem.Application.Tickets.Commands.CreateTicket;
using TicketSystem.Application.Tickets.Commands.DeleteTicket;
using TicketSystem.Application.Tickets.Commands.SubmitResolution;
using TicketSystem.Application.Tickets.Commands.UpdateStatus;
using TicketSystem.Application.Tickets.Dtos;
using TicketSystem.Application.Tickets.Queries.GetTicket;
using TicketSystem.Application.Tickets.Queries.GetTicketList;

namespace TicketSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpPost("createTicket")]
    [Authorize]
    public async Task<ActionResult<long>> CreateTicket(CreateTicketCommand command, CancellationToken cancellationToken)
    {
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
        // command = command with { CurrentUserId = User.GetUserId() };
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("updateStatus")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(UpdateTicketStatusCommand command, CancellationToken cancellationToken)
    {
        // command = command with { CurrentUserId = User.GetUserId() };
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpGet("list")]
    [Authorize]
    public async Task<ActionResult<PagedList<TicketDto>>> GetTickets(
    [FromQuery] GetTicketListQuery query,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("submitResolution")]
    [Authorize]
    public async Task<IActionResult> SubmitResolution(SubmitResolutionCommand command, CancellationToken cancellationToken)
    {
        command = command with { OperatorId = User.GetUserId() };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("close")]
    public async Task<IActionResult> Close(CloseTicketCommand command, CancellationToken cancellationToken)
    {
        command = command with { OperatorId = User.GetUserId() };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var command = new DeleteTicketCommand(TicketId: id, OperatorId: User.GetUserId());
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("audits")]
    public async Task<IActionResult> GetAudits(GetTicketAuditsQuery query, CancellationToken cancellationToken)
    {
        var audits = await mediator.Send(query, cancellationToken);
        return Ok(audits);
    }
}