using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using TicketSystem.Application.Comments.Commands.CreateComment;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.UnitTests.Comments.Commands.CreateComment;

public class CreateCommentCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Associate_Comment_With_Ticket_And_Current_User()
    {
        var ticket = new Ticket { Id=10, Title="Payment issue" };
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Tickets).ReturnsDbSet([ticket]);
        TicketComment? added=null;
        db.Setup(x => x.TicketComments.Add(It.IsAny<TicketComment>())).Callback<TicketComment>(c => { added=c; c.Id=88; });
        db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var current = new Mock<ICurrentUser>();
        current.SetupGet(x => x.UserId).Returns(42);
        var handler = new CreateCommentCommandHandler(db.Object, current.Object);

        var id = await handler.Handle(new CreateCommentCommand(10, "Investigating", true), CancellationToken.None);

        id.Should().Be(88);
        added.Should().NotBeNull();
        added!.TicketId.Should().Be(10);
        added.CommenterId.Should().Be(42);
        added.Content.Should().Be("Investigating");
        added.IsInternal.Should().BeTrue();
        db.Verify(x => x.TicketComments.Add(It.Is<TicketComment>(c => c.TicketId==10 && c.CommenterId==42)), Times.Once);
        db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Reject_Missing_Ticket_Without_Persisting()
    {
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Tickets).ReturnsDbSet(Array.Empty<Ticket>());
        var current = new Mock<ICurrentUser>();
        current.SetupGet(x => x.UserId).Returns(42);
        var handler = new CreateCommentCommandHandler(db.Object, current.Object);

        var act = async () => await handler.Handle(new CreateCommentCommand(999, "No ticket", false), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>()
            .Where(x => x.ErrorCode == ErrorCodeEnum.TicketNotFound.ToString());
        db.Verify(x => x.TicketComments.Add(It.IsAny<TicketComment>()), Times.Never);
        db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Surface_SaveFailure()
    {
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Tickets).ReturnsDbSet([new Ticket { Id=10 }]);
        db.Setup(x => x.TicketComments).ReturnsDbSet(Array.Empty<TicketComment>());
        db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("database unavailable"));
        var current = new Mock<ICurrentUser>();
        current.SetupGet(x => x.UserId).Returns(42);
        var handler = new CreateCommentCommandHandler(db.Object, current.Object);

        var act = async () => await handler.Handle(new CreateCommentCommand(10, "Comment", false), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("database unavailable");
    }
}