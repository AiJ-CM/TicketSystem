using TicketSystem.Domain.Enums;

namespace TicketSystem.Domain.Entities;

public class Ticket
{
    public long Id { get; set; }
    public string TicketNo { get; set; } = string.Empty;  // 工单编号
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    // 关联用户
    public int CreatorId { get; set; }
    public int? AssigneeId { get; set; }
    public string TicketType { get; set; } = "IT_SUPPORT";
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public User Creator { get; set; } = null!;
    public User? Assignee { get; set; }
}