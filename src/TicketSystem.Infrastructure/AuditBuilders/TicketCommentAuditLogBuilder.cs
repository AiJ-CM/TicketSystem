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
                audit.BusinessNote = $"添加了评论 (内部: {comment.IsInternal})";
                break;

            default:
                return null!;
        }

        return audit;
    }

    public void AttachAudit(TicketAudit audit, EntityEntry entry)
    {
        // 优先走外键，彻底避免导航属性为 null 的问题
        if (entry.Entity is TicketComment comment)
        {
            audit.TicketId = comment.TicketId;
            // 如果导航加载了，也挂进去双重保险
            comment.Ticket?.Audits.Add(audit);
        }
    }
}