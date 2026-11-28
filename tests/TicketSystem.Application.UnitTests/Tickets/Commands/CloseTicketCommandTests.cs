using TicketSystem.Application.Tickets.Commands.CloseTicket;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class CloseTicketCommandTests
{
    [Fact]
    public void Constructor_Should_Retain_TicketId()
    {
        var command = new CloseTicketCommand(31);

        command.TicketId.Should().Be(31);
    }
}
