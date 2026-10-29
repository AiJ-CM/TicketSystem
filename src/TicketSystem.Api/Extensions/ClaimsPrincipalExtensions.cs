using System.Security.Claims;
using TicketSystem.Application.Common.Exceptions;

namespace TicketSystem.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (claim == null || !long.TryParse(claim, out var userId))
            throw new BusinessException("当前用户身份凭证无效，请重新登录。", ErrorCodeEnum.Unauthorized);

        return userId;
    }

    public static string GetUserName(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrEmpty(claim))
            throw new BusinessException("当前用户缺少用户名凭证，请重新登录。", ErrorCodeEnum.Unauthorized);
        return claim;
    }
}