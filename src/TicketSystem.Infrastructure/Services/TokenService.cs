using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Infrastructure.Configuration;

namespace TicketSystem.Infrastructure.Services;

public class TokenService(JwtSettings settings, TimeProvider timeProvider) : ITokenService
{
    public string CreateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(settings.SecretKey));

        var expires = timeProvider.GetUtcNow().UtcDateTime
            .AddMinutes(settings.ExpiresMinutes);

        // JWT 只保留身份所需的基础信息；权限由服务端从 Redis/数据库动态校验。
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}