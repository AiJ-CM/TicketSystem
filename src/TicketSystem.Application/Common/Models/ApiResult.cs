namespace TicketSystem.Application.Common.Models;

public class ApiResult<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string? RequestId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResult<T> Success(T data, string message = "操作成功")
        => new() { Code = 200, Message = message, Data = data };

    public static ApiResult<T> Fail(int code, string message, string? requestId = null)
        => new() { Code = code, Message = message, Data = default, RequestId = requestId };

    public static ApiResult<T> NotFound(string message = "资源不存在", string? requestId = null)
        => new() { Code = 404, Message = message, Data = default, RequestId = requestId };

    public static ApiResult<T> BadRequest(string message, string? requestId = null)
        => new() { Code = 400, Message = message, Data = default, RequestId = requestId };
}

public class ApiResult : ApiResult<object> { }