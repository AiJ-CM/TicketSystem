using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Infrastructure.AuditBuilders;

public sealed class AuditBuilderFactory : IAuditBuilderFactory
{
    private readonly Dictionary<Type, IAuditLogBuilder> _builders;

    public AuditBuilderFactory(IEnumerable<IAuditLogBuilder> builders)
    {
        _builders = [];
        foreach (var builder in builders)
        {
            _builders[builder.GetSupportedType()] = builder;
        }
    }

    public void BuildAudit(EntityEntry entry, long operatorId)
    {
        var entityType = entry.Entity.GetType();

        if (_builders.TryGetValue(entityType, out var builder))
        {
            var audit = builder.BuildAuditLog(entry, operatorId);
            if (audit is not null)
            {
                builder.AttachAudit(audit, entry);
            }
        }
    }
}