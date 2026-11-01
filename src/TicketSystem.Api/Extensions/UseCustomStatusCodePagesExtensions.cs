namespace TicketSystem.Api.Extensions;

public static class UseCustomStatusCodePagesExtensions
{
    public static IApplicationBuilder UseCustomStatusCodePages(this IApplicationBuilder app)
    {
        return app.UseStatusCodePages(async context =>
        {
            var httpContext = context.HttpContext;
            var response = httpContext.Response;

            if (response.ContentLength.HasValue && response.ContentLength > 0
                || response.Body.Length > 0)
                return;

            response.ContentType = "application/json";

            var statusCode = response.StatusCode;
            var (code, message) = statusCode switch
            {
                400 => ("bad_request", "请求参数无效"),
                401 => ("unauthorized", "未认证或令牌无效"),
                403 => ("forbidden", "无权访问该资源"),
                404 => ("not_found", "请求的资源不存在"),
                405 => ("method_not_allowed", "请求方法不被允许"),
                415 => ("unsupported_media_type", "不支持的媒体类型"),
                429 => ("too_many_requests", "请求过于频繁，请稍后重试"),
                500 => ("internal_server_error", "服务器内部错误"),
                _ => ("error", "请求处理失败")
            };

            var body = System.Text.Json.JsonSerializer.Serialize(new
            {
                statusCode,
                error = message,
                details = message,
                path = httpContext.Request.Path.Value,
                method = httpContext.Request.Method
            });

            await response.WriteAsync(body);
        });
    }
}
