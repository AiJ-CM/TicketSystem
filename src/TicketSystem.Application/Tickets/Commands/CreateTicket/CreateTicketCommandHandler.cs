using MediatR;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler(
    ITicketingDbContext context) : IRequestHandler<CreateTicketCommand, long>
{
    public async Task<long> Handle(
        CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            TicketType = request.TicketType,
            Priority = request.Priority,
            CreatorId = request.CreatorId,
            Status = TicketStatus.Open
        };

        context.Tickets.Add(ticket);
        await context.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}