using Microsoft.AspNetCore.Authorization;

namespace TicketSystem.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "permission:";

    public RequirePermissionAttribute(string permission)
        : base($"{PolicyPrefix}{permission}")
    {
    }
}
