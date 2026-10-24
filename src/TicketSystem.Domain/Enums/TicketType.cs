namespace TicketSystem.Domain.Enums;

public enum TicketType
{
    /// <summary>
    /// 故障报错 (如：系统崩溃、功能不可用)
    /// </summary>
    Bug,

    /// <summary>
    /// 功能需求 (如：希望增加某项新特性)
    /// </summary>
    FeatureRequest,

    /// <summary>
    /// 业务咨询/提问
    /// </summary>
    Question,

    /// <summary>
    /// 账户或数据异常
    /// </summary>
    AccountData,

    /// <summary>
    /// 其他
    /// </summary>
    Other
}