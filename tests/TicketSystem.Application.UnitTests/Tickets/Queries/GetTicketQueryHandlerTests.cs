using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Queries.GetTicket;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Queries;

public class GetTicketQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Ticket_Details_With_Creator_And_Assignee()
    {
        // Arrange
        var assignee = new User { Id = 202, DisplayName = "Assigned agent" };
        var ticket = new Ticket
        {
            Id = 17,
            TicketNo = "TK-00017",
            Title = "Payment failed",
            Description = "Payment request timed out",
            Status = TicketStatus.InProgress,
            TicketType = TicketType.Bug,
            Priority = TicketPriority.High,
            CreatedBy = "Ticket owner",
            CreatorId = 101,
            AssigneeId = assignee.Id,
            Assignee = assignee
        };
        var database = new Mock<ITicketingDbContext>();
        database.Setup(context => context.Tickets).ReturnsDbSet([ticket]);
        var handler = new GetTicketQueryHandler(database.Object);

        // Act
        var result = await handler.Handle(new GetTicketQuery(ticket.Id), CancellationToken.None);

        // Assert
        result.Id.Should().Be(ticket.Id);
        result.TicketNo.Should().Be("TK-00017");
        result.Title.Should().Be("Payment failed");
        result.Description.Should().Be("Payment request timed out");
        result.Status.Should().Be(nameof(TicketStatus.InProgress));
        result.TicketType.Should().Be(nameof(TicketType.Bug));
        result.Priority.Should().Be(nameof(TicketPriority.High));
        result.CreatorName.Should().Be("Ticket owner");
        result.AssigneeName.Should().Be("Assigned agent");
    }

    [Fact]
    public async Task Handle_Should_Return_Null_Assignee_When_Ticket_Is_Unassigned()
    {
        // Arrange
        var ticket = new Ticket
        {
            Id = 18,
            Title = "Unassigned ticket",
            AssigneeId = null,
            Assignee = null
        };
        var database = new Mock<ITicketingDbContext>();
        database.Setup(context => context.Tickets).ReturnsDbSet([ticket]);
        var handler = new GetTicketQueryHandler(database.Object);

        // Act
        var result = await handler.Handle(new GetTicketQuery(ticket.Id), CancellationToken.None);

        // Assert
        result.Id.Should().Be(ticket.Id);
        result.Title.Should().Be("Unassigned ticket");
        result.AssigneeId.Should().BeNull();
        result.AssigneeName.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_Throw_BusinessException_When_Ticket_Does_Not_Exist()
    {
        // Arrange
        const long missingTicketId = 404L;
        var database = new Mock<ITicketingDbContext>();
        database.Setup(context => context.Tickets).ReturnsDbSet([]);
        var handler = new GetTicketQueryHandler(database.Object);

        // Act
        var act = () => handler.Handle(new GetTicketQuery(missingTicketId), CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<BusinessException>();
        exception.Which.ErrorCode.Should().Be(ErrorCodeEnum.TicketNotFound.ToString());
        exception.Which.Message.Should().Contain(missingTicketId.ToString());
    }
}
