using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
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
        var ticket = new Ticket { Id = 61, Status = TicketStatus.Open };
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
        var ticket = new Ticket { Id = 61, Status = TicketStatus.Closed };
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
