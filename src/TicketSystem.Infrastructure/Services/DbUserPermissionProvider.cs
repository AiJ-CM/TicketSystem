using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Infrastructure.Services;

public class DbUserPermissionProvider(ITicketingDbContext db) : IUserPermissionProvider
{
    public async Task<IReadOnlyCollection<string>> GetPermissionCodesAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        // 从用户角色及角色下的权限表联查，数据库结果去重后用于 Redis 回写。
        var permissionCodes = await db.Users
            .AsNoTracking()
            .Where(user => user.Id == userId && user.IsActive)
            .SelectMany(user => user.Roles)
            .SelectMany(role => role.Permissions)
            .Where(permission => !permission.IsDeleted)
            .Select(permission => permission.PermissionCode)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        return permissionCodes;
    }
}
