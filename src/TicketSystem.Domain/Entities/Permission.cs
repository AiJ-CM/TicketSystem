using TicketSystem.Domain.Entities.Common;

namespace TicketSystem.Domain.Entities;

public class Permission : BaseEntity
{
    public string PermissionCode { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Role> Roles { get; set; } = [];
}
