using MediatR;

namespace TicketSystem.Application.Tickets.Commands.AssignTicket;

public record AssignTicketCommand(
    long TicketId,
    int CurrentUserId,
    int AssigneeId
) : IRequest;