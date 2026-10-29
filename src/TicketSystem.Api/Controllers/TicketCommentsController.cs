using MediatR;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Comments.Commands.CreateComment;
using TicketSystem.Application.Comments.Queries.GetComments;

namespace TicketSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var query = new GetCommentsByTicketIdQuery(ticketId);
        var comments = await mediator.Send(query, cancellationToken);
        return Ok(comments);
    }
}