using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace TicketSystem.Api.Authorization;

public class DynamicPolicyProvider(
    IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(
                RequirePermissionAttribute.PolicyPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return base.GetPolicyAsync(policyName);
        }

        // 策略名中去除前缀后就是权限标识，例如 permission:ticket:create。
        var permission = policyName[RequirePermissionAttribute.PolicyPrefix.Length..];
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permission))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
