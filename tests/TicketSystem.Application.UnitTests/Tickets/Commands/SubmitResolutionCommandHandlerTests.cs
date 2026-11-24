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
        var ticket = new Ticket { Id = 51, Status = TicketStatus.InProgress };
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        var handler = new SubmitResolutionCommandHandler(context.Object);

        await handler.Handle(new SubmitResolutionCommand(51, "修复完成"), CancellationToken.None);

        ticket.Status.Should().Be(TicketStatus.Resolved);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Reject_Missing_Ticket_Without_Save()
    {
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet(Array.Empty<Ticket>());
        var handler = new SubmitResolutionCommandHandler(context.Object);

        var act = () => handler.Handle(new SubmitResolutionCommand(404, "修复完成"), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("未找到指定的工单");
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Reject_Invalid_Status_Without_Mutation()
    {
        var ticket = new Ticket { Id = 51, Status = TicketStatus.Closed };
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        var handler = new SubmitResolutionCommandHandler(context.Object);

        var act = () => handler.Handle(new SubmitResolutionCommand(51, "修复完成"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>().Where(e => e.ErrorCode == ErrorCodeEnum.InvalidStatusTransition.ToString());
        ticket.Status.Should().Be(TicketStatus.Closed);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
