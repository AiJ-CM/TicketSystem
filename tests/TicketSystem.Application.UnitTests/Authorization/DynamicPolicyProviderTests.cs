using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using TicketSystem.Api.Authorization;

namespace TicketSystem.Application.UnitTests.Authorization;

public class DynamicPolicyProviderTests
{
    [Fact]
    public async Task GetPolicyAsync_Should_Build_Permission_Policy_With_Requirement()
    {
        var policy = await CreateProvider().GetPolicyAsync("permission:ticket:create");
        policy.Should().NotBeNull();
        policy!.AuthenticationSchemes.Should().BeEmpty();
        policy.Requirements.OfType<PermissionRequirement>().Should().ContainSingle();
        var requirement = policy.Requirements.OfType<PermissionRequirement>().Single();
        requirement.Permission.Should().Be("ticket:create");
    }

    [Fact]
    public async Task GetPolicyAsync_Should_Match_Prefix_Without_Case_Sensitivity()
    {
        var policy = await CreateProvider().GetPolicyAsync("PERMISSION:Ticket.Update");
        policy.Should().NotBeNull();
        policy!.Requirements.OfType<PermissionRequirement>().Single().Permission.Should().Be("Ticket.Update");
    }

    [Fact]
    public async Task GetPolicyAsync_Should_Fall_Back_For_Unrelated_Policy_Name()
    {
        var policy = await CreateProvider().GetPolicyAsync("not-a-dynamic-policy");
        policy.Should().BeNull();
    }

    [Fact]
    public async Task GetPolicyAsync_Should_Produce_Equivalent_Policies_On_Repeated_Lookups()
    {
        var provider = CreateProvider();
        var first = await provider.GetPolicyAsync("permission:ticket:close");
        var second = await provider.GetPolicyAsync("permission:ticket:close");
        first.Should().NotBeNull();
        second.Should().NotBeNull();
        var firstRequirement = first!.Requirements.OfType<PermissionRequirement>().Single();
        var secondRequirement = second!.Requirements.OfType<PermissionRequirement>().Single();
        secondRequirement.Permission.Should().Be(firstRequirement.Permission);
    }

    [Fact]
    public async Task DefaultAndFallbackPolicies_Should_Return_Configured_Framework_Behavior()
    {
        var provider = CreateProvider();
        var defaultPolicy = await provider.GetDefaultPolicyAsync();
        var fallbackPolicy = await provider.GetFallbackPolicyAsync();
        defaultPolicy.Should().NotBeNull();
        defaultPolicy.Requirements.OfType<DenyAnonymousAuthorizationRequirement>().Should().ContainSingle();
        fallbackPolicy.Should().BeNull();
    }

    private static DynamicPolicyProvider CreateProvider()
        => new(Options.Create(new AuthorizationOptions()));
}