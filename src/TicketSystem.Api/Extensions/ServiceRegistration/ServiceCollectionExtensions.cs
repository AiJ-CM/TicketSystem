namespace TicketSystem.Api.Extensions.ServiceRegistration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAllServices(
         this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddPresentationServices()
            .AddApplicationServices()
            .AddInfrastructureServices(configuration);

        return services;
    }
}