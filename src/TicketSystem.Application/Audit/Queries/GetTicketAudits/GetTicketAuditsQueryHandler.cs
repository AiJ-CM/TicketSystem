using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Audit.Dtos;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Audit.Queries.GetTicketAudits;

public class GetTicketAuditsQueryHandler(ITicketingDbContext context) : IRequestHandler<GetTicketAuditsQuery, IEnumerable<AuditDto>>
{
    public async Task<IEnumerable<AuditDto>> Handle(GetTicketAuditsQuery query, CancellationToken ct)
    {
        return await context.TicketAudits
            .AsNoTracking()
            .Where(a => a.TicketId == query.TicketId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AuditDto() { Action = a.Action.ToString(),OperatorName ="", CreatedAt = a.CreatedAt})
            .ToListAsync(ct);
    }
}