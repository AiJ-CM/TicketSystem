using MediatR;
using TicketSystem.Application.Tickets.Dtos;

namespace TicketSystem.Application.Tickets.Queries.GetTicket;

public record GetTicketQuery(long Id) : IRequest<TicketDto>;