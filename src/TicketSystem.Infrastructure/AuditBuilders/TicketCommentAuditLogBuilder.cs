using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Infrastructure.AuditBuilders;

public class TicketCommentAuditLogBuilder : IAuditLogBuilder
{
    public Type GetSupportedType() => typeof(TicketComment);

    public TicketAudit? BuildAuditLog(EntityEntry entry, long operatorId)
    {
        var comment = entry.Entity as TicketComment
            ?? throw new InvalidOperationException("TicketCommentAuditLogBuilder 仅适用于 TicketComment 实体");

        if (entry.State != EntityState.Added) return null;

        var audit = new TicketAudit
        {
            TicketId = comment.TicketId,
            OperatorId = operatorId,
            CreatedAt = DateTime.UtcNow,
            Action = TicketAuditAction.AddedComment,
            BusinessNote = comment.IsInternal switch
            {
                true => "添加了内部评论",
                false => "添加了评论"
            }
        };

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