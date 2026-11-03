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