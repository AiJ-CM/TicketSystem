using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Commands.UpdateStatus;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class UpdateTicketStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Allowed_Status_And_Save()
    {
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 61)
            .With(x => x.Status, TicketStatus.Open)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        var handler = new UpdateTicketStatusCommandHandler(context.Object);

        await handler.Handle(new UpdateTicketStatusCommand(61, TicketStatus.InProgress), CancellationToken.None);

        ticket.Status.Should().Be(TicketStatus.InProgress);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Reject_Disallowed_Transition_Without_Mutation()
    {
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 61)
            .With(x => x.Status, TicketStatus.Closed)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        var handler = new UpdateTicketStatusCommandHandler(context.Object);

        var act = () => handler.Handle(new UpdateTicketStatusCommand(61, TicketStatus.Open), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.InvalidStatusTransition.ToString());
        ticket.Status.Should().Be(TicketStatus.Closed);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Reject_Missing_Ticket_Without_Save()
    {
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet(Array.Empty<Ticket>());
        var handler = new UpdateTicketStatusCommandHandler(context.Object);

        var act = () => handler.Handle(new UpdateTicketStatusCommand(404, TicketStatus.InProgress), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.TicketNotFound.ToString());
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
