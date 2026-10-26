using TicketSystem.Domain.Entities.Common;

namespace TicketSystem.Domain.Entities;

public class TicketComment : AuditEntity
{
    public long TicketId { get; set; }
    public long CommenterId { get; set; }

    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public User Commenter { get; set; } = null!;
}