using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TicketSystem.Infrastructure.AuditBuilders;

public interface IAuditBuilderFactory
{
    void BuildAudit(EntityEntry entry, long operatorId);
}