namespace TicketSystem.Domain.Enums;

public enum TicketStatus
{
    Open = 1,          // 待分配
    InProgress = 2,    // 处理中
    PendingUser = 3,   // 等待申请人回复
    Resolved = 4,      // 已解决
    Closed = 5,        // 已关闭
    Reopened = 6       // 已重开
}