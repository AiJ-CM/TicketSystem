namespace TicketSystem.Application.Audit.Dtos;

public class AuditDto
{
    public long Id{ get; set; }
    public long TicketId{ get; set; }
    public long OperatorId{ get; set; }
    public required string OperatorName{ get; set; }
    public required string Action { get; set; }
    public string? OldValue{ get; set; }
    public string? NewValue{ get; set; }
    public string? BusinessNote{ get; set; }
    public DateTime CreatedAt { get; set; }
}
