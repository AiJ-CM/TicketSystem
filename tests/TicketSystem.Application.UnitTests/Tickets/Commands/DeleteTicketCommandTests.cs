using TicketSystem.Application.Tickets.Commands.DeleteTicket;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class DeleteTicketCommandTests
{
    [Fact]
    public void Constructor_Should_Retain_TicketId()
    {
        var command = new DeleteTicketCommand(41);

        command.TicketId.Should().Be(41);
    }
}
