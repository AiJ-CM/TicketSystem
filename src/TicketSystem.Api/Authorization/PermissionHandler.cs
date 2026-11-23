using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Api.Authorization;

public class PermissionHandler(
    IHttpContextAccessor httpContextAccessor,
    IDistributedCache cache,
    IUserPermissionProvider permissionProvider) : AuthorizationHandler<PermissionRequirement>
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        // 权限变更后，最多约一分钟内生效；可按部署要求缩短为秒级。
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    };

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var user = httpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var nameIdentifier = user.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(nameIdentifier, out var userId))
        {
            return;
        }

        var cacheKey = $"ticket-system:permissions:{userId}";
        var cachedPermissions = await cache.GetStringAsync(cacheKey);
        IReadOnlyCollection<string> permissions;

        if (!string.IsNullOrEmpty(cachedPermissions))
        {
            // Redis 命中时直接使用缓存，不再读取 JWT 中的权限 Claims。
            permissions = JsonSerializer.Deserialize<string[]>(cachedPermissions) ?? [];
        }
        else
        {
            // Redis 未命中时从数据库查询用户角色及权限，并回写缓存。
            permissions = await permissionProvider.GetPermissionCodesAsync(
                userId,
                httpContext?.RequestAborted ?? CancellationToken.None);

            await cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(permissions),
                CacheOptions,
                httpContext?.RequestAborted ?? CancellationToken.None);
        }

        if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}
