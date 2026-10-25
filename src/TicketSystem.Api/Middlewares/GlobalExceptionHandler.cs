using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using TicketSystem.Application.Common.Exceptions;

namespace TicketSystem.Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var requestId = httpContext.TraceIdentifier
            ?? Guid.NewGuid().ToString("N")[..8];

        logger.LogError(
            exception,
            "Request {RequestId} failed at {Time} with {StatusCode}",
            requestId,
            DateTime.UtcNow,
            exception switch
            {
                BusinessException => 400,
                ValidationException => 400,
                _ => 500
            });

        httpContext.Response.ContentType = "application/json";

        var (statusCode, errorCode, userMessage, details) = exception switch
        {
            BusinessException bizEx => (
                StatusCodes.Status400BadRequest,
                bizEx.ErrorCode ?? "BusinessError",
                bizEx.Message,
                bizEx.InnerException?.Message
            ),
            ValidationException valEx => (
                StatusCodes.Status400BadRequest,
                "ValidationError",
                "请求参数校验失败",
                string.Join("; ", valEx.Errors.Select(e => e.ErrorMessage))
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "InternalServerError",
                "服务器内部错误，请稍后重试",
                null
            )
        };

        httpContext.Response.StatusCode = statusCode;

        var errorResponse = new
        {
            success = false,
            code = statusCode,
            requestId,
            errorCode,
            message = userMessage,
            details,
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        await httpContext.Response.WriteAsync(
          JsonSerializer.Serialize(errorResponse, JsonOptions), cancellationToken);

        return true;
    }
}