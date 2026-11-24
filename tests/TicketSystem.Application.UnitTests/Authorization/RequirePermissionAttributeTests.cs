using FluentAssertions;
using TicketSystem.Api.Authorization;

namespace TicketSystem.Application.UnitTests.Authorization;

public class RequirePermissionAttributeTests
{
    [Fact]
    public void Constructor_Should_Generate_Permission_Policy_Name()
    {
        var attribute = new RequirePermissionAttribute("ticket:create");
        attribute.Policy.Should().Be("permission:ticket:create");
    }

    [Fact]
    public void Constructor_Should_Preserve_Empty_Permission_In_Policy_Name()
    {
        var attribute = new RequirePermissionAttribute(string.Empty);
        attribute.Policy.Should().Be("permission:");
    }
}