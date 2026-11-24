using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using TicketSystem.Application.Comments.Queries.GetComments;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.UnitTests.Comments.Queries.GetComments;

public class GetCommentsByTicketIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Only_Requested_Ticket_Comments_Newest_First()
    {
        var commenter = new User { Id=5, UserName="alice", DisplayName="Alice" };
        var older = new TicketComment { Id=1, TicketId=10, CommenterId=5, Commenter=commenter, Content="Older", IsInternal=false, CreatedAt=new DateTime(2026,1,1), CreatedBy="alice" };
        var newer = new TicketComment { Id=2, TicketId=10, CommenterId=5, Commenter=commenter, Content="Newer", IsInternal=true, CreatedAt=new DateTime(2026,1,2), CreatedBy="alice" };
        var other = new TicketComment { Id=3, TicketId=11, CommenterId=5, Commenter=commenter, Content="Other ticket", CreatedAt=new DateTime(2026,1,3), CreatedBy="alice" };
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.TicketComments).ReturnsDbSet([older, newer, other]);
        var handler = new GetCommentsByTicketIdQueryHandler(db.Object);

        var result = (await handler.Handle(new GetCommentsByTicketIdQuery(10), CancellationToken.None)).ToList();

        result.Should().HaveCount(2);
        result.Select(x => x.Id).Should().ContainInOrder(2, 1);
        result[0].CommenterId.Should().Be(5);
        result[0].CommenterName.Should().Be("Alice");
        result[0].Content.Should().Be("Newer");
        result[0].IsInternal.Should().BeTrue();
        result[0].CreatedBy.Should().Be("alice");
        result.Should().NotContain(x => x.Id == 3);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_When_No_Comments_Exist()
    {
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.TicketComments).ReturnsDbSet(Array.Empty<TicketComment>());
        var handler = new GetCommentsByTicketIdQueryHandler(db.Object);

        var result = await handler.Handle(new GetCommentsByTicketIdQuery(10), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_Exclude_Comments_From_Other_Tickets()
    {
        var commenter = new User { Id=5, UserName="alice" };
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.TicketComments).ReturnsDbSet([
            new TicketComment { Id=20, TicketId=11, CommenterId=5, Commenter=commenter, Content="Wrong", CreatedBy="alice" }
        ]);
        var handler = new GetCommentsByTicketIdQueryHandler(db.Object);

        var result = await handler.Handle(new GetCommentsByTicketIdQuery(10), CancellationToken.None);

        result.Should().BeEmpty();
    }
}