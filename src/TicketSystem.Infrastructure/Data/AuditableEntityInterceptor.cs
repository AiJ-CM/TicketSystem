using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Entities.Common;
using TicketSystem.Infrastructure.AuditBuilders;

namespace TicketSystem.Infrastructure.Data;

public class AuditableEntityInterceptor(ICurrentUser currentUser, IAuditBuilderFactory auditFactory) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is TicketingDbContext ticketContext)
        {
            var allEntries = ticketContext.ChangeTracker.Entries().ToList();
            if (allEntries?.Count > 0 && allEntries.Any(e => e.Entity is not TicketAudit && e.Entity is not User))
            {
                // 统一上下文：一次保存内时间戳、用户完全一致
                var now = DateTime.UtcNow;
                var user = currentUser.UserName;
                var userId = currentUser.UserId;

                UpdateAuditableEntities(allEntries, user, now);
                CreateTicketAudit(allEntries, userId);
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateAuditableEntities(List<EntityEntry> entries, string user, DateTime now)
    {
        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity baseEntity && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                baseEntity.IsDeleted = true;
                baseEntity.DeletedAt = now;
                baseEntity.DeletedBy = user;
            }

            if (entry.Entity is AuditEntity auditEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    auditEntity.CreatedAt = now;
                    auditEntity.CreatedBy = user;
                    auditEntity.UpdatedAt = now;
                    auditEntity.UpdatedBy = user;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditEntity.UpdatedAt = now;
                    auditEntity.UpdatedBy = user;
                }
            }
        }
    }

    private void CreateTicketAudit(List<EntityEntry> entries, long userId)
    {
        foreach (var entry in entries)
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
            {
                continue;
            }

            if (entry.Entity is TicketAudit)
            {
                continue;
            }

            auditFactory.BuildAudit(entry, userId);
        }
    }
}