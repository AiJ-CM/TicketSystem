using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Common.Interfaces;

public interface IAuditLogBuilder
{
    Type GetSupportedType();
    TicketAudit BuildAuditLog(EntityEntry entry, long operatorId);
    void AttachAudit(TicketAudit audit, EntityEntry entry);
}