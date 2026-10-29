using System.Security.Claims;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Api.Extensions;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public long UserId => User.GetUserId();

    public string UserName => User.GetUserName();

    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new BusinessException("当前HTTP上下文或用户为空", ErrorCodeEnum.Unauthorized);
}