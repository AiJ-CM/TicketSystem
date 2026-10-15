using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Infrastructure.Configuration;
using TicketSystem.Infrastructure.Data;
using TicketSystem.Infrastructure.Services;

namespace TicketSystem.Api.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. 绑定 JWT 配置并注册
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
        services.AddSingleton(jwtSettings);

        // 2. 注册数据库
        services.AddDbContext<TicketingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TicketDb")));

        // 3. 注册自定义数据库接口
        services.AddScoped<ITicketingDbContext>(sp =>
            sp.GetRequiredService<TicketingDbContext>());

        // 4. 注册 Token 服务
        services.AddScoped<ITokenService, TokenService>();

        // 5. 注册 Http Context 访问器
        services.AddHttpContextAccessor();

        // 6. 注册 JWT 认证
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        return services;
    }
}