using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Commands.CloseTicket;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class CloseTicketCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Close_Closable_Ticket_And_Save()
    {
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 31)
            .With(x => x.Status, TicketStatus.Resolved)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        var handler = new CloseTicketCommandHandler(context.Object);

        await handler.Handle(new CloseTicketCommand(31), CancellationToken.None);

        ticket.Status.Should().Be(TicketStatus.Closed);
        ticket.ClosedAt.Should().NotBeNull();
        context.Verify(x => x.Tickets.Update(ticket), Times.Once);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Reject_Missing_Ticket_Without_Saving()
    {
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet(Array.Empty<Ticket>());
        var handler = new CloseTicketCommandHandler(context.Object);

        var act = () => handler.Handle(new CloseTicketCommand(404), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.TicketNotFound.ToString());
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Reject_Disallowed_Status_Without_Mutation()
    {
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 31)
            .With(x => x.Status, TicketStatus.Closed)
            .With(x => x.ClosedAt, (DateTime?)null)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        var handler = new CloseTicketCommandHandler(context.Object);

        var act = () => handler.Handle(new CloseTicketCommand(31), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.StatusNotChanged.ToString());
        ticket.Status.Should().Be(TicketStatus.Closed);
        ticket.ClosedAt.Should().BeNull();
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
