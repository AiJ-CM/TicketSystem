using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Common.Interfaces;

public interface IAuditLogService
{
    /// <summary>
    /// 记录工单操作日志
    /// </summary>
    Task LogAsync(long ticketId, TicketAuditAction action, string note);
}