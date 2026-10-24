using TicketSystem.Domain.Entities.Common;

namespace TicketSystem.Domain.Entities;

public class TicketComment : BaseEntity
{
    public long Id { get; set; }
    public long TicketId { get; set; }
    public int CommenterId { get; set; }

    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public User Commenter { get; set; } = null!;
}