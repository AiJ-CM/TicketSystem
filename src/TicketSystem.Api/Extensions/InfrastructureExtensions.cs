using Microsoft.EntityFrameworkCore;

namespace TicketSystem.Api.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. 注册 EF Core 和 SQL Server
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // 2. 注册 Azure Key Vault (如果在本地开发，使用 User Secrets)
        configuration.AddAzureKeyVault(
            new Uri("https://your-keyvault-name.vault.azure.net/"),
            new DefaultAzureCredential()
        );

        return services;
    }
}