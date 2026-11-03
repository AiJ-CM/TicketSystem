using System.Text.Json;
using TicketSystem.Application.Common.Models;

namespace TicketSystem.Api.Extensions;

public static class UseCustomStatusCodePagesExtensions
{
    public static IApplicationBuilder UseCustomStatusCodePages(this IApplicationBuilder app)
    {
        return app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;

            if (response.HasStarted)
                return;

            response.ContentType = "application/json";

            var statusCode = (int)context.HttpContext.Response.StatusCode;
            var (code, message) = statusCode switch
            {
                404 => ("NotFound", "请求的资源不存在"),
                405 => ("MethodNotAllowed", "请求方法不被允许"),
                _ => throw new Exception("Unhandled status code in CustomStatusCodePages")
            };

            var requestId = context.HttpContext.TraceIdentifier;
            requestId ??= Guid.NewGuid().ToString("N")[..8];

            var responseModel = ApiResult<object>.Fail(statusCode, message, requestId);

            await response.WriteAsync(
                JsonSerializer.Serialize(responseModel),
                context.HttpContext.RequestAborted);
        });
    }
}