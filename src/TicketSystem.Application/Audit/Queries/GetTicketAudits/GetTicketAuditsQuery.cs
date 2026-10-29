using MediatR;
using TicketSystem.Application.Audit.Dtos;

namespace TicketSystem.Application.Audit.Queries.GetTicketAudits;

public record GetTicketAuditsQuery(long TicketId) : IRequest<List<AuditDto>>;