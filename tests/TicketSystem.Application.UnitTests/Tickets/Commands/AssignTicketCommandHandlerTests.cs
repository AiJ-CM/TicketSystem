using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Commands.AssignTicket;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class AssignTicketCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Assign_Existing_Ticket_And_Save()
    {
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 17)
            .With(x => x.AssigneeId, (long?)null)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var assignee = fixture.Build<User>()
            .With(x => x.Id, 29)
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        context.Setup(x => x.Users).ReturnsDbSet([assignee]);
        var handler = new AssignTicketCommandHandler(context.Object);

        await handler.Handle(new AssignTicketCommand(17, 29), CancellationToken.None);

        ticket.AssigneeId.Should().Be(29);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Reject_Missing_Ticket_Without_Saving()
    {
        var fixture = new Fixture();
        var assignee = fixture.Build<User>()
            .With(x => x.Id, 29)
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet(Array.Empty<Ticket>());
        context.Setup(x => x.Users).ReturnsDbSet([assignee]);
        var handler = new AssignTicketCommandHandler(context.Object);

        var act = () => handler.Handle(new AssignTicketCommand(404, 29), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.TicketNotFound.ToString());
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Reject_Already_Assigned_Without_Saving()
    {
        var fixture = new Fixture();
        var ticket = fixture.Build<Ticket>()
            .With(x => x.Id, 17)
            .With(x => x.AssigneeId, 29)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var assignee = fixture.Build<User>()
            .With(x => x.Id, 29)
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        context.Setup(x => x.Users).ReturnsDbSet([assignee]);
        var handler = new AssignTicketCommandHandler(context.Object);

        var act = () => handler.Handle(new AssignTicketCommand(17, 29), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.TicketAlreadyAssigned.ToString());
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
