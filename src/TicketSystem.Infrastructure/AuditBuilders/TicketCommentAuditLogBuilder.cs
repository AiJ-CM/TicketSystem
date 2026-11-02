using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Infrastructure.AuditBuilders;

public class TicketCommentAuditLogBuilder : IAuditLogBuilder
{
    public Type GetSupportedType() => typeof(TicketComment);

    public TicketAudit BuildAuditLog(EntityEntry entry, long operatorId)
    {
        var comment = entry.Entity as TicketComment
            ?? throw new InvalidOperationException("TicketCommentAuditLogBuilder 仅适用于 TicketComment 实体");

        var audit = new TicketAudit
        {
            TicketId = comment.TicketId,
            OperatorId = operatorId,
            CreatedAt = DateTime.UtcNow
        };

        switch (entry.State)
        {
            case EntityState.Added:
                audit.Action = TicketAuditAction.AddedComment;
                var internalStr = comment.IsInternal? "内部" : string.Empty;
                audit.BusinessNote = $"添加了{internalStr}评论";
                break;

            default:
                return null!;
        }

        return audit;
    }

    public void AttachAudit(TicketAudit audit, EntityEntry entry)
    {
        if (entry.Entity is TicketComment comment)
        {
            comment.Ticket?.Audits.Add(audit);
        }
    }
}