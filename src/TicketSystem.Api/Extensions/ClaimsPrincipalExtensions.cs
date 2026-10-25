using System.Security.Claims;
using TicketSystem.Application.Common.Exceptions;

namespace TicketSystem.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (claim == null || !int.TryParse(claim, out var userId))
            throw new BusinessException("当前用户身份凭证无效，请重新登录。", ErrorCodeEnum.Unauthorized);

        return userId;
    }
}