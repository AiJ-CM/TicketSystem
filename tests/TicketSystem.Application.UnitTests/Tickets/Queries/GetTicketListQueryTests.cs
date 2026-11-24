using FluentAssertions;
using TicketSystem.Application.Tickets.Queries.GetTicketList;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Queries;

public class GetTicketListQueryTests
{
    [Fact]
    public void Query_Should_Retain_Paging_Filter_And_Sort_Inputs()
    {
        var query = new GetTicketListQuery
        {
            PageIndex = 3,
            PageSize = 15,
            Status = TicketStatus.InProgress,
            AssignedTo = 29,
            CreatedBy = 31
        };

        query.PageIndex.Should().Be(3);
        query.PageSize.Should().Be(15);
        query.Status.Should().Be(TicketStatus.InProgress);
        query.AssignedTo.Should().Be(29);
        query.CreatedBy.Should().Be(31);
    }

    [Fact]
    public void Query_Should_Use_Default_Paging_When_Not_Set()
    {
        var query = new GetTicketListQuery();

        query.PageIndex.Should().Be(1);
        query.PageSize.Should().Be(20);
        query.Status.Should().BeNull();
        query.AssignedTo.Should().BeNull();
        query.CreatedBy.Should().BeNull();
    }
}
