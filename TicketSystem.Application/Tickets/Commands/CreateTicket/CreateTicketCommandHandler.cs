using MediatR;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, long>
{
    private readonly TicketingDbContext _context;

    public CreateTicketCommandHandler(TicketingDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            TicketType = request.TicketType,
            Priority = request.Priority,
            CreatorId = request.CreatorId,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}