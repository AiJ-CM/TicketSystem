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

    // 导航属性
    public ICollection<Ticket> CreatedTickets { get; set; } = [];
    public ICollection<Ticket> AssignedTickets { get; set; } = [];
}