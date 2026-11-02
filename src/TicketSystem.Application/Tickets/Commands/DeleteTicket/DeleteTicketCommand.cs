using MediatR;

namespace TicketSystem.Application.Tickets.Commands.DeleteTicket;

public record DeleteTicketCommand(long TicketId) : IRequest;