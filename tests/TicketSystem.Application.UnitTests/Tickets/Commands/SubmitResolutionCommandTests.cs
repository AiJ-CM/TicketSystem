using FluentAssertions;
using TicketSystem.Application.Tickets.Commands.SubmitResolution;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class SubmitResolutionCommandTests
{
    [Fact]
    public void Constructor_Should_Retain_Resolution_Inputs()
    {
        var command = new SubmitResolutionCommand(51, "已修复并回归验证");

        command.TicketId.Should().Be(51);
        command.ResolutionNote.Should().Be("已修复并回归验证");
    }
}
