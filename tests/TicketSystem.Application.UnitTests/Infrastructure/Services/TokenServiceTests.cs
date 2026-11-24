using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using TicketSystem.Domain.Entities;
using TicketSystem.Infrastructure.Configuration;
using TicketSystem.Infrastructure.Services;

namespace TicketSystem.Application.UnitTests.Infrastructure.Services;

public class TokenServiceTests
{
    [Fact]
    public void CreateToken_Should_Parse_With_Identity_Claims_And_No_Permissions()
    {
        var settings = CreateSettings();
        var sut = new TokenService(settings);
        var user = new User { Id = 42, UserName = "alice" };

        var token = sut.CreateToken(user);

        token.Should().NotBeNullOrWhiteSpace();
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var principal = handler.ValidateToken(token, CreateValidationParameters(settings), out var validatedToken);
        validatedToken.Should().BeOfType<JwtSecurityToken>();
        principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.Should().Be("42");
        principal.FindFirst(ClaimTypes.Name)?.Value.Should().Be("alice");
        principal.Claims.Should().NotContain(claim => claim.Type.Contains("permission", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CreateToken_Should_Use_Configured_Expiry()
    {
        var settings = CreateSettings(expiresMinutes: 45);
        var sut = new TokenService(settings);
        var beforeCreation = DateTime.UtcNow;

        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(sut.CreateToken(new User { Id = 7, UserName = "bob" }));

        parsed.ValidTo.Should().BeOnOrAfter(beforeCreation.AddMinutes(44));
        parsed.ValidTo.Should().BeOnOrBefore(DateTime.UtcNow.AddMinutes(46));
    }

    [Fact]
    public void CreateToken_Should_Fail_When_Signing_Key_Is_Missing()
    {
        var settings = CreateSettings();
        settings.SecretKey = string.Empty;
        var sut = new TokenService(settings);

        var act = () => sut.CreateToken(new User { Id = 8, UserName = "charlie" });

        act.Should().Throw<Exception>();
    }

    private static JwtSettings CreateSettings(int expiresMinutes = 30)
    {
        return new JwtSettings
        {
            SecretKey = "unit-test-secret-key-32-bytes-long!",
            Issuer = "TicketSystem.UnitTests",
            Audience = "TicketSystem.UnitTests",
            ExpiresMinutes = expiresMinutes
        };
    }

    private static TokenValidationParameters CreateValidationParameters(JwtSettings settings)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = settings.Issuer,
            ValidateAudience = true,
            ValidAudience = settings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}