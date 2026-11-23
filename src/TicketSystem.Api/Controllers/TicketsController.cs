using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Api.Authorization;
using TicketSystem.Application.Audit.Dtos;
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
[Authorize]
public class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpPost("createTicket")]
    [RequirePermission("ticket:create")]
    public async Task<ActionResult<long>> CreateTicket(CreateTicketCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTicket), new { id }, id);
    }

    [HttpGet("{id}")]
    [RequirePermission("ticket:read")]
    public async Task<ActionResult<TicketDto>> GetTicket(long id, CancellationToken cancellationToken)
    {
        var query = new GetTicketQuery(id);
        var ticketDto = await mediator.Send(query, cancellationToken);
        return Ok(ticketDto);
    }

    [HttpPost("assign")]
    [RequirePermission("ticket:assign")]
    public async Task<IActionResult> Assign(AssignTicketCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("updateStatus")]
    [RequirePermission("ticket:update")]
    public async Task<IActionResult> UpdateStatus(UpdateTicketStatusCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("list")]
    [RequirePermission("ticket:read")]
    public async Task<ActionResult<PagedList<TicketDto>>> GetTickets(GetTicketListQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("submitResolution")]
    [RequirePermission("ticket:update")]
    public async Task<IActionResult> SubmitResolution(SubmitResolutionCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("close")]
    [RequirePermission("ticket:update")]
    public async Task<IActionResult> Close(CloseTicketCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [RequirePermission("ticket:delete")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTicketCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpGet("audits/{id}")]
    [RequirePermission("ticket:audit")]
    public async Task<ActionResult<IEnumerable<AuditDto>>> GetAudits(long id, CancellationToken cancellationToken)
    {
        var audits = await mediator.Send(new GetTicketAuditsQuery(id), cancellationToken);
        return Ok(audits);
    }
}