using TicketSystem.Domain.Enums;

namespace TicketSystem.Domain.Entities;

public class TicketAudit
{
    public long Id { get; set; }
    public long TicketId { get; set; }
    public long OperatorId { get; set; }

    public TicketAuditAction Action { get; set; }

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? BusinessNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
    public User Operator { get; set; } = null!;
}
