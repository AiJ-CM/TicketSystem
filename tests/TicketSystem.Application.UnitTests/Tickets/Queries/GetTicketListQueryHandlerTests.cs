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
        var creator = new User { Id = 31, DisplayName = "创建者" };
        var assignee = new User { Id = 29, DisplayName = "处理人" };
        var matching = new Ticket { Id = 2, Title = "匹配工单", Status = TicketStatus.InProgress, CreatorId = 31, Creator = creator, AssigneeId = 29, Assignee = assignee, CreatedAt = new DateTime(2026, 1, 2) };
        var otherStatus = new Ticket { Id = 1, Title = "其他状态", Status = TicketStatus.Open, CreatorId = 31, Creator = creator, CreatedAt = new DateTime(2026, 1, 1) };
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([otherStatus, matching]);
        var handler = new GetTicketListQueryHandler(context.Object);

        var result = await handler.Handle(new GetTicketListQuery { PageIndex = 1, PageSize = 10, Status = TicketStatus.InProgress, AssignedTo = 29, CreatedBy = 31 }, CancellationToken.None);

        result.TotalItems.Should().Be(1);
        result.TotalPages.Should().Be(1);
        result.Items.Should().ContainSingle();
        result.Items[0].Id.Should().Be(2);
        result.Items[0].Title.Should().Be("匹配工单");
        result.Items[0].AssigneeName.Should().Be("处理人");
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_For_OutOfRange_Page()
    {
        var context = new Mock<ITicketingDbContext>();
        context.Setup(x => x.Tickets).ReturnsDbSet([new Ticket { Id = 1, Title = "唯一工单", Status = TicketStatus.Open, CreatorId = 31 }]);
        var handler = new GetTicketListQueryHandler(context.Object);

        var result = await handler.Handle(new GetTicketListQuery { PageIndex = 99, PageSize = 20 }, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(1);
        result.PageIndex.Should().Be(99);
        result.TotalPages.Should().Be(1);
    }
}
