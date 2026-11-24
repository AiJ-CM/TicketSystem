using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using TicketSystem.Api.Authorization;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.UnitTests.Authorization;

public class PermissionHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Succeed_When_Permission_Is_In_Cache()
    {
        var requirement = new PermissionRequirement("ticket:create");
        var principal = CreatePrincipal(42);
        var context = new AuthorizationHandlerContext([requirement], principal, null);
        var cache = new Mock<IDistributedCache>();
        var provider = new Mock<IUserPermissionProvider>();
        cache.Setup(x => x.GetAsync("ticket-system:permissions:42", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new[] { "ticket:read", "ticket:create" })));
        var handler = CreateHandler(principal, cache, provider);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
        provider.Verify(x => x.GetPermissionCodesAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
        cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_Load_From_Provider_And_Write_Cache_On_Miss()
    {
        var requirement = new PermissionRequirement("ticket:create");
        var principal = CreatePrincipal(42);
        var context = new AuthorizationHandlerContext([requirement], principal, null);
        var cache = new Mock<IDistributedCache>();
        var provider = new Mock<IUserPermissionProvider>();
        cache.Setup(x => x.GetAsync("ticket-system:permissions:42", It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);
        cache.Setup(x => x.SetAsync("ticket-system:permissions:42", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        provider.Setup(x => x.GetPermissionCodesAsync(42, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { "ticket:read", "ticket:create" });
        var handler = CreateHandler(principal, cache, provider);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
        provider.Verify(x => x.GetPermissionCodesAsync(42, It.IsAny<CancellationToken>()), Times.Once);
        cache.Verify(x => x.SetAsync("ticket-system:permissions:42", It.Is<byte[]>(bytes => Encoding.UTF8.GetString(bytes).Contains("ticket:create")), It.Is<DistributedCacheEntryOptions>(options => options.AbsoluteExpirationRelativeToNow == TimeSpan.FromSeconds(60)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_Not_Succeed_When_NameIdentifier_Is_Missing()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "Bearer"));
        var requirement = new PermissionRequirement("ticket:create");
        var context = new AuthorizationHandlerContext([requirement], principal, null);
        var cache = new Mock<IDistributedCache>();
        var provider = new Mock<IUserPermissionProvider>();
        var handler = CreateHandler(principal, cache, provider);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
        cache.VerifyNoOtherCalls();
        provider.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_Should_Not_Succeed_When_Permission_Is_Missing()
    {
        var requirement = new PermissionRequirement("ticket:delete");
        var principal = CreatePrincipal(42);
        var context = new AuthorizationHandlerContext([requirement], principal, null);
        var cache = new Mock<IDistributedCache>();
        var provider = new Mock<IUserPermissionProvider>();
        cache.Setup(x => x.GetAsync("ticket-system:permissions:42", It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new[] { "ticket:create", "ticket:read" })));
        var handler = CreateHandler(principal, cache, provider);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
        provider.Verify(x => x.GetPermissionCodesAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_Propagate_Provider_Failure_On_Cache_Miss()
    {
        var requirement = new PermissionRequirement("ticket:create");
        var principal = CreatePrincipal(42);
        var context = new AuthorizationHandlerContext([requirement], principal, null);
        var cache = new Mock<IDistributedCache>();
        var provider = new Mock<IUserPermissionProvider>();
        cache.Setup(x => x.GetAsync("ticket-system:permissions:42", It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);
        provider.Setup(x => x.GetPermissionCodesAsync(42, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("permission store unavailable"));
        var handler = CreateHandler(principal, cache, provider);

        var act = async () => await handler.HandleAsync(context);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("permission store unavailable");
    }

    [Fact]
    public async Task HandleAsync_Should_Not_Succeed_When_UserId_Is_Invalid()
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "not-a-user-id")], "Bearer");
        var principal = new ClaimsPrincipal(identity);
        var requirement = new PermissionRequirement("ticket:create");
        var context = new AuthorizationHandlerContext([requirement], principal, null);
        var cache = new Mock<IDistributedCache>();
        var provider = new Mock<IUserPermissionProvider>();
        var handler = CreateHandler(principal, cache, provider);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
        cache.VerifyNoOtherCalls();
        provider.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_Should_Not_Succeed_When_User_Is_Not_Authenticated()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "42")], null));
        var requirement = new PermissionRequirement("ticket:create");
        var context = new AuthorizationHandlerContext([requirement], principal, null);
        var cache = new Mock<IDistributedCache>();
        var provider = new Mock<IUserPermissionProvider>();
        var handler = CreateHandler(principal, cache, provider);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
        cache.VerifyNoOtherCalls();
        provider.VerifyNoOtherCalls();
    }
    private static ClaimsPrincipal CreatePrincipal(long userId)
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())], "Bearer");
        return new ClaimsPrincipal(identity);
    }

    private static PermissionHandler CreateHandler(ClaimsPrincipal principal, Mock<IDistributedCache> cache, Mock<IUserPermissionProvider> provider)
    {
        var httpContext = new DefaultHttpContext { User = principal };
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        return new PermissionHandler(accessor, cache.Object, provider.Object);
    }
}