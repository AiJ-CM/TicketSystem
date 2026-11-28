using TicketSystem.Api.Filters;
using TicketSystem.Api.Middlewares;
using TicketSystem.Api.Transformers;

namespace TicketSystem.Api.Extensions.ServiceRegistration;

public static class PresentationExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ApiResultFilter>();
        });

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // 动态策略提供器：遇到 permission:* 策略时按权限标识实时生成策略。
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        services.AddAuthorization();

        // 优先使用 Redis；未配置 Redis 时使用进程内分布式缓存作为开发回退。
        var redisConfiguration = configuration.GetValue<string>("Redis:Configuration");
        if (!string.IsNullOrWhiteSpace(redisConfiguration))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfiguration;
                options.InstanceName = "TicketSystem:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
                       ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }
}