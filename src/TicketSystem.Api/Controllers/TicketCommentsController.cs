using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Comments.Commands.CreateComment;
using TicketSystem.Application.Comments.Queries.GetComments;

namespace TicketSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketCommentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateComment(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetComments), new { ticketId = command.TicketId }, new { id });
    }

    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetComments(long ticketId, CancellationToken cancellationToken)
    {
        var comments = await mediator.Send(new GetCommentsByTicketIdQuery(ticketId), cancellationToken);
        return Ok(comments);
    }
}