using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Models;

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
        var requestId = httpContext.TraceIdentifier ?? Guid.NewGuid().ToString("N")[..8];

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

        var (statusCode, errorCode, userMessage) = exception switch
        {
            BusinessException bizEx => (
                StatusCodes.Status400BadRequest,
                bizEx.ErrorCode ?? "BusinessError",
                bizEx.Message
            ),
            ValidationException valEx => (
                StatusCodes.Status400BadRequest,
                "ValidationError",
                string.Join("; ", valEx.Errors.Select(e => e.ErrorMessage))
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "InternalServerError",
                "服务器内部错误，请稍后重试"
            )
        };

        httpContext.Response.StatusCode = statusCode;

        var response = ApiResult<object>.Fail(statusCode, userMessage, requestId);

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions), cancellationToken);

        return true;
    }
}