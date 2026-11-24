using FluentAssertions;
using TicketSystem.Application.Tickets.Commands.CreateTicket;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class CreateTicketCommandTests
{
    [Fact]
    public void Constructor_Should_Retain_All_Ticket_Inputs()
    {
        // Arrange
        const string title = "Payment failed";
        const string description = "The payment request timed out.";

        // Act
        var command = new CreateTicketCommand(
            title,
            description,
            TicketType.Bug,
            TicketPriority.High);

        // Assert
        command.Title.Should().Be(title);
        command.Description.Should().Be(description);
        command.TicketType.Should().Be(TicketType.Bug);
        command.Priority.Should().Be(TicketPriority.High);
    }
}