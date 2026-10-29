namespace TicketSystem.Domain.Enums;

public enum TicketAuditAction
{
    /// <summary>
    /// 创建工单
    /// </summary>
    Created,

    /// <summary>
    /// 更新工单信息（如标题、描述）
    /// </summary>
    Updated,

    /// <summary>
    /// 指派/转交工单
    /// </summary>
    Assigned,

    /// <summary>
    /// 状态变更
    /// </summary>
    StatusChanged,

    /// <summary>
    /// 关闭工单
    /// </summary>
    Closed,

    /// <summary>
    /// 重新打开工单
    /// </summary>
    Reopened,

    /// <summary>
    /// 添加内部/外部评论
    /// </summary>
    AddedComment,

    /// <summary>
    /// 删除工单
    /// </summary>
    Deleted,

    /// <summary>
    /// 解决工单
    /// </summary>
    Resolved
}