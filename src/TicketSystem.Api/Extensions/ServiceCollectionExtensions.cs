namespace TicketSystem.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 注册 MediatR
        // services.AddMediatR(...); 
        // 注册业务应用层服务
        return services;
    }
}