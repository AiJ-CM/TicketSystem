using FluentAssertions;
using TicketSystem.Application.Tickets.Commands.AssignTicket;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class AssignTicketCommandTests
{
    [Fact]
    public void Constructor_Should_Retain_Assignment_Values()
    {
        var command = new AssignTicketCommand(17, 29);

        command.TicketId.Should().Be(17);
        command.AssigneeId.Should().Be(29);
    }
}
