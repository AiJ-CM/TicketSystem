using FluentAssertions;
using TicketSystem.Api.Authorization;

namespace TicketSystem.Application.UnitTests.Authorization;

public class PermissionRequirementTests
{
    [Fact]
    public void Constructor_Should_Retain_Permission_Exactly()
    {
        var requirement = new PermissionRequirement("Ticket.Create");
        requirement.Permission.Should().Be("Ticket.Create");
    }

    [Fact]
    public void Constructor_Should_Accept_Empty_Permission()
    {
        var requirement = new PermissionRequirement(string.Empty);
        requirement.Permission.Should().BeEmpty();
    }
}