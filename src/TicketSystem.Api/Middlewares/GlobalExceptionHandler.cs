using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace TicketSystem.Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. 记录异常日志
        logger.LogError(exception, "Unhandled exception occurred at {Time}", DateTime.UtcNow);

        httpContext.Response.ContentType = "application/json";

        // 2. 根据异常类型分类，返回不同的状态码
        var statusCode = exception switch
        {
            ValidationException ex => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        // 3. 统一错误响应格式
        var errorResponse = new
        {
            statusCode,
            error = GetErrorMessage(statusCode),
            details = exception.Message,
            path = httpContext.Request.Path.Value,
            method = httpContext.Request.Method
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(errorResponse), cancellationToken);

        return true;
    }

    /// <summary>
    /// 根据状态码返回友好的错误提示
    /// </summary>
    private static string GetErrorMessage(int statusCode) => statusCode switch
    {
        400 => "Request parameter or business logic error",
        401 => "Unauthorized, please log in first",
        404 => "Requested resource not found",
        _ => "Internal server error, please try again later"
    };
}