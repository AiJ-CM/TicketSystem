using TicketSystem.Domain.Entities.Common;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Domain.Entities;

public class User : BaseEntity
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Ticket> CreatedTickets { get; set; } = [];
    public ICollection<Ticket> AssignedTickets { get; set; } = [];
    public ICollection<TicketAudit> Audits { get; set; } = [];
    public ICollection<TicketComment> Comments { get; set; } = [];
}