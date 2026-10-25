namespace TicketSystem.Application.Common.Exceptions;

public class BusinessException : Exception
{
    /// <summary>
    /// 业务错误码，前端可按码做逻辑判断
    /// </summary>
    public string ErrorCode { get; }

    public BusinessException(string message, ErrorCodeEnum errorCode = ErrorCodeEnum.Unknown)
        : base(message)
    {
        ErrorCode = errorCode.ToString();
    }
}