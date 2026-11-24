using FluentAssertions;
using TicketSystem.Application.Comments.Queries.GetComments;

namespace TicketSystem.Application.UnitTests.Comments.Queries.GetComments;

public class GetCommentsByTicketIdQueryTests
{
    [Fact]
    public void Constructor_Should_Retain_TicketId()
    {
        var query = new GetCommentsByTicketIdQuery(275);
        query.TicketId.Should().Be(275);
    }
}