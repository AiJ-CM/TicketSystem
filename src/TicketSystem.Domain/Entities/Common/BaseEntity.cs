namespace TicketSystem.Domain.Entities.Common;

public abstract class BaseEntity
{
    public long Id { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
