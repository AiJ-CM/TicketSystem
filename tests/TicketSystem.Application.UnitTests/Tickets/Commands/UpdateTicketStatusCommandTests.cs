using TicketSystem.Application.Tickets.Commands.UpdateStatus;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class UpdateTicketStatusCommandTests
{
    [Fact]
    public void Constructor_Should_Retain_Status_Inputs()
    {
        var command = new UpdateTicketStatusCommand(61, TicketStatus.InProgress);

        command.TicketId.Should().Be(61);
        command.NewStatus.Should().Be(TicketStatus.InProgress);
    }
}
