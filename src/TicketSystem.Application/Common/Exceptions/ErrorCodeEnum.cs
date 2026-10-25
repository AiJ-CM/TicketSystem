namespace TicketSystem.Application.Common.Exceptions;

public enum ErrorCodeEnum
{
    // 系统/通用异常
    Unauthorized,
    Forbidden,
    NotFound,
    Unknown,

    // 用户相关
    UserNotFound,
    UserCodeDuplicate,
    EmailDuplicate,
    LoginFailed,

    // 工单相关
    TicketNotFound,
    InvalidTicketState,
    StatusNotChanged,
    InvalidStatusTransition
}