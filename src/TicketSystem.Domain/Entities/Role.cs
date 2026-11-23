using TicketSystem.Domain.Entities.Common;

namespace TicketSystem.Domain.Entities;

public class Role : BaseEntity
{
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; } = [];
}
