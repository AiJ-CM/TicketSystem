using AutoFixture;
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
        var fixture = new Fixture();
        var commenter = fixture.Build<User>()
            .With(x => x.Id, 5)
            .With(x => x.UserName, "alice")
            .With(x => x.DisplayName, "Alice")
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var older = fixture.Build<TicketComment>()
            .With(x => x.Id, 1)
            .With(x => x.TicketId, 10)
            .With(x => x.CommenterId, 5)
            .With(x => x.Commenter, commenter)
            .With(x => x.Content, "Older")
            .With(x => x.IsInternal, false)
            .With(x => x.CreatedAt, new DateTime(2026, 1, 1))
            .With(x => x.CreatedBy, "alice")
            .Without(x => x.Ticket)
            .Create();
        var newer = fixture.Build<TicketComment>()
            .With(x => x.Id, 2)
            .With(x => x.TicketId, 10)
            .With(x => x.CommenterId, 5)
            .With(x => x.Commenter, commenter)
            .With(x => x.Content, "Newer")
            .With(x => x.IsInternal, true)
            .With(x => x.CreatedAt, new DateTime(2026, 1, 2))
            .With(x => x.CreatedBy, "alice")
            .Without(x => x.Ticket)
            .Create();
        var other = fixture.Build<TicketComment>()
            .With(x => x.Id, 3)
            .With(x => x.TicketId, 11)
            .With(x => x.CommenterId, 5)
            .With(x => x.Commenter, commenter)
            .With(x => x.Content, "Other ticket")
            .With(x => x.IsInternal, false)
            .With(x => x.CreatedAt, new DateTime(2026, 1, 3))
            .With(x => x.CreatedBy, "alice")
            .Without(x => x.Ticket)
            .Create();
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
        var fixture = new Fixture();
        var commenter = fixture.Build<User>()
            .With(x => x.Id, 5)
            .With(x => x.UserName, "alice")
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.TicketComments).ReturnsDbSet([
            fixture.Build<TicketComment>()
                .With(x => x.Id, 20)
                .With(x => x.TicketId, 11)
                .With(x => x.CommenterId, 5)
                .With(x => x.Commenter, commenter)
                .With(x => x.Content, "Wrong")
                .With(x => x.CreatedBy, "alice")
                .Without(x => x.Ticket)
                .Create()
        ]);
        var handler = new GetCommentsByTicketIdQueryHandler(db.Object);

        var result = await handler.Handle(new GetCommentsByTicketIdQuery(10), CancellationToken.None);

        result.Should().BeEmpty();
    }
}