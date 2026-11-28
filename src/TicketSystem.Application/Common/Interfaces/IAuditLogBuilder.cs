using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TicketSystem.Application.Common.Interfaces;

public interface IAuditLogBuilder
{
    Type GetSupportedType();
    TicketAudit? BuildAuditLog(EntityEntry entry, long operatorId);
    void AttachAudit(TicketAudit audit, EntityEntry entry);
}