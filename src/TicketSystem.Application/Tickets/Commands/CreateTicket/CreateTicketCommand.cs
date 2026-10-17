using MediatR;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(
    string Title,
    string Description,
    string TicketType,
    TicketPriority Priority,
    int CreatorId) : IRequest<long>;