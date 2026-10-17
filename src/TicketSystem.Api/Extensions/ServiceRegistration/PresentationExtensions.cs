using TicketSystem.Api.Middlewares;
using TicketSystem.Api.Transformers;

namespace TicketSystem.Api.Extensions.ServiceRegistration;

public static class PresentationExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }
}