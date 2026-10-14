using MediatR;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommand : IRequest<long>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TicketType { get; set; } = "IT_SUPPORT";
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public int CreatorId { get; set; }
}