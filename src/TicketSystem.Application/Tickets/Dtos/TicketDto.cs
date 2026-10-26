namespace TicketSystem.Application.Tickets.Dtos;

public class TicketDto
{
    // --- BaseEntity 基类字段 ---
    public long Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    // --- Ticket 业务字段 ---
    public string TicketNo { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string TicketType { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public DateTime? AssignedAt { get; init; }
    public DateTime? ClosedAt { get; init; }

    // --- 关联用户信息 ---
    public string? CreatorName { get; init; }
    public int? AssigneeId { get; init; }
    public string? AssigneeName { get; init; }
}