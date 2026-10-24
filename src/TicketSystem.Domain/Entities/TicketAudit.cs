using TicketSystem.Domain.Entities.Common;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Domain.Entities;

public class TicketAudit : BaseEntity
{
    public long Id { get; set; }
    public long TicketId { get; set; }
    public int OperatorId { get; set; }

    public TicketAuditAction Action { get; set; }

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? BusinessNote { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public User Operator { get; set; } = null!;
}
