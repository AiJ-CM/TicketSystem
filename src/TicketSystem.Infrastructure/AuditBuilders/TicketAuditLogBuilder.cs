using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Infrastructure.AuditBuilders;

public class TicketAuditLogBuilder(TimeProvider timeProvider) : IAuditLogBuilder
{
    public Type GetSupportedType() => typeof(Ticket);

    public TicketAudit? BuildAuditLog(EntityEntry entry, long operatorId)
    {
        var ticket = entry.Entity as Ticket
            ?? throw new InvalidOperationException("TicketAuditLogBuilder 仅适用于 Ticket 实体");

        return (entry.State, entry) switch
        {
            // 新增工单
            (EntityState.Added, _) => BuildAudit(a =>
            {
                a.Action = TicketAuditAction.Created;
                a.BusinessNote = $"创建了工单（操作人ID: {operatorId}）";
            }),

            // 状态变更
            (EntityState.Modified, var e) when e.Property(nameof(Ticket.Status)).IsModified =>
                BuildAudit(a =>
                {
                    var statusProp = e.Property(nameof(Ticket.Status));
                    var oldStatus = statusProp.OriginalValue?.ToString() ?? "未知";

                    a.Action = TicketAuditAction.StatusChanged;
                    a.OldValue = oldStatus;
                    a.NewValue = ticket.Status.ToString();
                    a.BusinessNote = $"工单《{ticket.Title}》状态由 {oldStatus} 变更为 {ticket.Status}";
                }),

            // 指派人变更
            (EntityState.Modified, var e) when e.Property(nameof(Ticket.AssigneeId)).IsModified =>
                BuildAudit(a =>
                {
                    var assigneeProp = e.Property(nameof(Ticket.AssigneeId));
                    var oldAssigneeId = assigneeProp.OriginalValue as long?;

                    a.Action = TicketAuditAction.Assigned;
                    a.OldValue = oldAssigneeId?.ToString();
                    a.NewValue = ticket.AssigneeId.ToString();
                    a.BusinessNote = oldAssigneeId.HasValue
                        ? $"将工单从 '用户ID：{oldAssigneeId}' 重新指派给了 '用户ID：{ticket.AssigneeId}'"
                        : $"将工单指派给了 '用户ID：{ticket.AssigneeId}'";
                }),

            _ => null
        };

        TicketAudit BuildAudit(Action<TicketAudit> configure)
        {
            var audit = new TicketAudit
            {
                TicketId = ticket.Id,
                OperatorId = operatorId,
                CreatedAt = timeProvider.GetUtcNow().UtcDateTime
            };

            configure(audit);

            return audit;
        }
    }

    public void AttachAudit(TicketAudit audit, EntityEntry entry)
    {
        if (entry.Entity is Ticket ticket)
        {
            ticket.Audits.Add(audit);
        }
    }
}