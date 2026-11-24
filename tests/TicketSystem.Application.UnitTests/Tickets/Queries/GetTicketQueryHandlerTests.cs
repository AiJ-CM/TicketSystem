using AutoFixture;
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
        var fixture = new Fixture();
        var assignee = fixture.Build<User>()
            .With(x => x.Id, 202)
            .With(x => x.DisplayName, "Assigned agent")
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 17)
            .With(x => x.TicketNo, "TK-00017")
            .With(x => x.Title, "Payment failed")
            .With(x => x.Description, "Payment request timed out")
            .With(x => x.Status, TicketStatus.InProgress)
            .With(x => x.TicketType, TicketType.Bug)
            .With(x => x.Priority, TicketPriority.High)
            .With(x => x.CreatedBy, "Ticket owner")
            .With(x => x.CreatorId, 101)
            .With(x => x.AssigneeId, (long?)202)
            .With(x => x.Assignee, assignee)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
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
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 18)
            .With(x => x.Title, "Unassigned ticket")
            .With(x => x.AssigneeId, (long?)null)
            .With(x => x.Assignee, (User?)null)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
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
