using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Infrastructure.AuditBuilders;

public class TicketAuditLogBuilder : IAuditLogBuilder
{
    public Type GetSupportedType() => typeof(Ticket);

    public TicketAudit BuildAuditLog(EntityEntry entry, long operatorId)
    {
        var ticket = entry.Entity as Ticket
            ?? throw new InvalidOperationException("TicketAuditLogBuilder 仅适用于 Ticket 实体");

        var audit = new TicketAudit
        {
            OperatorId = operatorId,
            CreatedAt = DateTime.UtcNow
        };

        switch (entry.State)
        {
            case EntityState.Added:
                audit.Action = TicketAuditAction.Created;
                audit.BusinessNote = $"创建了工单（操作人ID: {operatorId}）";
                break;

            case EntityState.Modified:
                if (entry.Property("Status").IsModified)
                {
                    var oldStatus = entry.Property("Status").OriginalValue?.ToString() ?? "未知";
                    var newStatus = ticket.Status;

                    audit.Action = TicketAuditAction.StatusChanged;
                    audit.OldValue = oldStatus;
                    audit.NewValue = newStatus.ToString();
                    audit.BusinessNote = $"工单《{ticket.Title}》状态由 {oldStatus} 变更为 {newStatus}";
                }
                else
                {
                    return null!;
                }
                break;

            default:
                return null!;
        }

        return audit;
    }

    public void AttachAudit(TicketAudit audit, EntityEntry entry)
    {
        if (entry.Entity is Ticket ticket)
        {
            ticket.Audits.Add(audit);
        }
    }
}