using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Infrastructure.Services;

public class AuditLogService(ITicketingDbContext context, ICurrentUser currentUser) : IAuditLogService
{
    public async Task LogAsync(long ticketId, TicketAuditAction action, string note)
    {
        var audit = new TicketAudit
        {
            TicketId = ticketId,
            OperatorId = currentUser.UserId,
            Action = action,
            BusinessNote = note,
            CreatedAt = DateTime.UtcNow
        };

        await context.TicketAudits.AddAsync(audit);
    }
}