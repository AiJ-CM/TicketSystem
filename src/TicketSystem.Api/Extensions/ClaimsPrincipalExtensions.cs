using System.Security.Claims;

namespace TicketSystem.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (claim == null || !int.TryParse(claim, out var userId))
            throw new UnauthorizedAccessException("Authentication failed. Please log in again.");

        return userId;
    }
}