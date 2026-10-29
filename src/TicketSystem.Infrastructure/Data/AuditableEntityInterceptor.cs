using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities.Common;

namespace TicketSystem.Infrastructure.Data;

public class AuditableEntityInterceptor(ICurrentUser? currentUser) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        var now = DateTime.UtcNow;

        // 1. 安全获取当前用户
        var user = currentUser?.UserName ?? "System";

        // 2. 软删除逻辑
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = now;
                entry.Entity.DeletedBy = user;
            }
        }

        // 3. 审计逻辑
        foreach (var entry in context.ChangeTracker.Entries<AuditEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = user;
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = user;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = user;
            }
        }
    }
}