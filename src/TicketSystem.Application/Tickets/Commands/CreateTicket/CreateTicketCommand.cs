using MediatR;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(
    string Title,
    string Description,
    TicketType TicketType,
    TicketPriority Priority
) : IRequest<long>;