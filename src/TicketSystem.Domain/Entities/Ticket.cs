using TicketSystem.Domain.Entities.Common;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Domain.Entities;

public class Ticket : AuditEntity
{
    public string TicketNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    // 关联用户
    public long CreatorId { get; set; }
    public long? AssigneeId { get; set; }
    public TicketType TicketType { get; set; }
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public DateTime? AssignedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public User Creator { get; set; } = null!;
    public User? Assignee { get; set; }
    public ICollection<TicketComment> Comments { get; set; } = [];
    public ICollection<TicketAudit> Audits { get; set; } = [];
}