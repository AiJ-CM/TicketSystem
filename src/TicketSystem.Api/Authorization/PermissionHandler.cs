using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Frozen;
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

    // 有界分段锁避免同一进程内相同权限缓存键并发回源。
    private static readonly SemaphoreSlim[] CacheLocks = [.. Enumerable.Range(0, 256).Select(_ => new SemaphoreSlim(1, 1))];

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
        var cancellationToken = httpContext?.RequestAborted ?? CancellationToken.None;
        var cachedPermissions = await cache.GetStringAsync(cacheKey, cancellationToken);
        FrozenSet<string> permissions;

        if (!string.IsNullOrEmpty(cachedPermissions))
        {
            // Redis 命中时直接使用缓存，不再读取 JWT 中的权限 Claims。
            permissions = DeserializePermissions(cachedPermissions);
        }
        else
        {
            var lockIndex = (int)((uint)StringComparer.Ordinal.GetHashCode(cacheKey) % (uint)CacheLocks.Length);
            var cacheLock = CacheLocks[lockIndex];
            await cacheLock.WaitAsync(cancellationToken);

            try
            {
                // 等待锁期间其他请求可能已完成回源，因此需要再次检查缓存。
                cachedPermissions = await cache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedPermissions))
                {
                    permissions = DeserializePermissions(cachedPermissions);
                }
                else
                {
                    // Redis 未命中时从数据库查询用户角色及权限，并回写缓存。
                    var permissionCodes = await permissionProvider.GetPermissionCodesAsync(userId, cancellationToken);
                    permissions = permissionCodes.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
                    await cache.SetStringAsync(
                        cacheKey,
                        JsonSerializer.Serialize(permissions),
                        CacheOptions,
                        cancellationToken);
                }
            }
            finally
            {
                cacheLock.Release();
            }
        }

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }

    private static FrozenSet<string> DeserializePermissions(string cachedPermissions)
    {
        var values = JsonSerializer.Deserialize<string[]>(cachedPermissions);
        return values is null
            ? FrozenSet<string>.Empty
            : values.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }
}
