using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Audit.Dtos;
using TicketSystem.Application.Audit.Mappers;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Audit.Queries.GetTicketAudits;

public class GetTicketAuditsQueryHandler(ITicketingDbContext context)
    : IRequestHandler<GetTicketAuditsQuery, IEnumerable<AuditDto>>
{
    public async Task<IEnumerable<AuditDto>> Handle(GetTicketAuditsQuery query, CancellationToken ct)
    {
        var ticketAudits = await context.TicketAudits
            .AsNoTracking()
            .Include(a => a.Ticket)
            .Include(a => a.Operator)
            .Where(a => a.TicketId == query.TicketId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

        var auditDtos = ticketAudits.Select(AuditMapper.ToAuditDto).ToList();

        return auditDtos;
    }
}