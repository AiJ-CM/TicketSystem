using MediatR;

namespace TicketSystem.Application.Tickets.Commands.CloseTicket;

public record CloseTicketCommand(
    long TicketId,
    long OperatorId
) : IRequest;
