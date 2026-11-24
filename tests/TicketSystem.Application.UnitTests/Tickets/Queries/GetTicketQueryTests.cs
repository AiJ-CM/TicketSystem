using FluentAssertions;
using TicketSystem.Application.Tickets.Queries.GetTicket;

namespace TicketSystem.Application.UnitTests.Tickets.Queries;

public class GetTicketQueryTests
{
    [Fact]
    public void Constructor_Should_Retain_Ticket_Id()
    {
        // Arrange
        const long ticketId = 42L;

        // Act
        var query = new GetTicketQuery(ticketId);

        // Assert
        query.Id.Should().Be(ticketId);
    }
}