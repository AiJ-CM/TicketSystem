using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Commands.DeleteTicket;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class DeleteTicketCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Remove_Existing_Ticket_And_Save()
    {
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 41)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        var handler = new DeleteTicketCommandHandler(context.Object);

        await handler.Handle(new DeleteTicketCommand(41), CancellationToken.None);

        context.Verify(x => x.Tickets.Remove(ticket), Times.Once);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Reject_Missing_Or_Deleted_Ticket_Without_Save()
    {
        var fixture = new Fixture();
        var deleted = fixture.Build<Ticket>()
            .With(x => x.Id, 41)
            .With(x => x.IsDeleted, true)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([deleted]);
        var handler = new DeleteTicketCommandHandler(context.Object);

        var act = () => handler.Handle(new DeleteTicketCommand(41), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("????????");
        context.Verify(x => x.Tickets.Remove(It.IsAny<Ticket>()), Times.Never);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
