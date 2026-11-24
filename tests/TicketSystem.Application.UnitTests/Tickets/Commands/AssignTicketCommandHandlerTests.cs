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
        var ticket = new Ticket { Id = 17, AssigneeId = null };
        var assignee = new User { Id = 29, DisplayName = "运维员" };
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
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet(Array.Empty<Ticket>());
        context.Setup(x => x.Users).ReturnsDbSet([new User { Id = 29 }]);
        var handler = new AssignTicketCommandHandler(context.Object);

        var act = () => handler.Handle(new AssignTicketCommand(404, 29), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.TicketNotFound.ToString());
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Reject_Already_Assigned_Without_Saving()
    {
        var ticket = new Ticket { Id = 17, AssigneeId = 29 };
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        context.Setup(x => x.Users).ReturnsDbSet([new User { Id = 29 }]);
        var handler = new AssignTicketCommandHandler(context.Object);

        var act = () => handler.Handle(new AssignTicketCommand(17, 29), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.TicketAlreadyAssigned.ToString());
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
