using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Queries.GetTicketList;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Queries;

public class GetTicketListQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Filter_Page_And_Report_Metadata()
    {
        var fixture = new Fixture();
        var creator = fixture.Build<User>()
            .With(x => x.Id, 31)
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var assignee = fixture.Build<User>()
            .With(x => x.Id, 29)
            .With(x => x.DisplayName, "???")
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var matching = fixture.Build<Ticket>()
            .With(x => x.Id, 2)
            .With(x => x.Title, "????")
            .With(x => x.Status, TicketStatus.InProgress)
            .With(x => x.CreatorId, 31)
            .With(x => x.Creator, creator)
            .With(x => x.AssigneeId, (long?)29)
            .With(x => x.Assignee, assignee)
            .With(x => x.CreatedAt, new DateTime(2026, 1, 2))
            .With(x => x.IsDeleted, false)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var otherStatus = fixture.Build<Ticket>()
            .With(x => x.Id, 1)
            .With(x => x.Title, "????")
            .With(x => x.Status, TicketStatus.Open)
            .With(x => x.CreatorId, 31)
            .With(x => x.Creator, creator)
            .With(x => x.AssigneeId, (long?)null)
            .With(x => x.Assignee, (User?)null)
            .With(x => x.CreatedAt, new DateTime(2026, 1, 1))
            .With(x => x.IsDeleted, false)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([otherStatus, matching]);
        var handler = new GetTicketListQueryHandler(context.Object);

        var result = await handler.Handle(new GetTicketListQuery { PageIndex = 1, PageSize = 10, Status = TicketStatus.InProgress, AssignedTo = 29, CreatedBy = 31 }, CancellationToken.None);

        result.TotalItems.Should().Be(1);
        result.TotalPages.Should().Be(1);
        result.Items.Should().ContainSingle();
        result.Items[0].Id.Should().Be(2);
        result.Items[0].Title.Should().Be("????");
        result.Items[0].AssigneeName.Should().Be("???");
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_For_OutOfRange_Page()
    {
        var fixture = new Fixture();
        var onlyTicket = fixture.Build<Ticket>()
            .With(x => x.Id, 1)
            .With(x => x.Title, "????")
            .With(x => x.Status, TicketStatus.Open)
            .With(x => x.CreatorId, 31)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Creator)
            .Without(x => x.Assignee)
            .Without(x => x.Comments)
            .Without(x => x.Audits)
            .Create();
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([onlyTicket]);
        var handler = new GetTicketListQueryHandler(context.Object);

        var result = await handler.Handle(new GetTicketListQuery { PageIndex = 99, PageSize = 20 }, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(1);
        result.PageIndex.Should().Be(99);
        result.TotalPages.Should().Be(1);
    }
}
