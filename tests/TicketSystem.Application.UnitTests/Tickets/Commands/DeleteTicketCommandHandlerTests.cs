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
        var ticket = new Ticket { Id = 41, IsDeleted = false };
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
        var deleted = new Ticket { Id = 41, IsDeleted = true };
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([deleted]);
        var handler = new DeleteTicketCommandHandler(context.Object);

        var act = () => handler.Handle(new DeleteTicketCommand(41), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("未找到指定的工单");
        context.Verify(x => x.Tickets.Remove(It.IsAny<Ticket>()), Times.Never);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
