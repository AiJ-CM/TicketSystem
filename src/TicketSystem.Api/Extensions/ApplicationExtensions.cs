using FluentValidation;
using TicketSystem.Api.Controllers;
using TicketSystem.Application.Common.Behaviors;
using TicketSystem.Application.Tickets.Commands.CreateTicket;

namespace TicketSystem.Api.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 自动扫描 Application 程序集里的所有 Validator
        services.AddValidatorsFromAssembly(
            typeof(ValidationBehavior<,>).Assembly);

        // MediatR + 验证管道
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(CreateTicketCommandHandler).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });


        return services;
    }
}
