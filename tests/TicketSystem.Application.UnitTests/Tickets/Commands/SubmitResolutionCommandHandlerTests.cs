using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Commands.SubmitResolution;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class SubmitResolutionCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Resolve_Eligible_Ticket_And_Save()
    {
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 51)
            .With(x => x.Status, TicketStatus.InProgress)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        var handler = new SubmitResolutionCommandHandler(context.Object);

        await handler.Handle(new SubmitResolutionCommand(51, "????"), CancellationToken.None);

        ticket.Status.Should().Be(TicketStatus.Resolved);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Reject_Missing_Ticket_Without_Save()
    {
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet(Array.Empty<Ticket>());
        var handler = new SubmitResolutionCommandHandler(context.Object);

        var act = () => handler.Handle(new SubmitResolutionCommand(404, "????"), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("????????");
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Reject_Invalid_Status_Without_Mutation()
    {
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 51)
            .With(x => x.Status, TicketStatus.Closed)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        var handler = new SubmitResolutionCommandHandler(context.Object);

        var act = () => handler.Handle(new SubmitResolutionCommand(51, "????"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.InvalidStatusTransition.ToString());
        ticket.Status.Should().Be(TicketStatus.Closed);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
