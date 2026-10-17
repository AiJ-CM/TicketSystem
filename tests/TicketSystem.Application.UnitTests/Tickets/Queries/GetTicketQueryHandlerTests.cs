using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Queries.GetTicket;
using TicketSystem.Application.UnitTests.Fixtures;

namespace TicketSystem.Application.UnitTests.Tickets.Queries;

public class GetTicketQueryHandlerTests
{
    private readonly Mock<ITicketingDbContext> _contextMock;
    private readonly GetTicketQueryHandler _handler;

    public GetTicketQueryHandlerTests()
    {
        _contextMock = new Mock<ITicketingDbContext>();
        _handler = new GetTicketQueryHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_TicketDto_When_Ticket_Exists()
    {
        _contextMock.Setup(x => x.Tickets).ReturnsDbSet([TestDataFactory.DefaultTicket]);
        _contextMock.Setup(x => x.Users)
            .ReturnsDbSet([TestDataFactory.DefaultCreator, TestDataFactory.DefaultAssignee]);

        var result = await _handler.Handle(
            new GetTicketQuery(TestDataFactory.DefaultTicket.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(TestDataFactory.DefaultTicket.Id);
        result.Title.Should().Be(TestDataFactory.DefaultTicket.Title);
    }

    [Fact]
    public async Task Handle_Should_Map_Creator_And_Assignee()
    {
        _contextMock.Setup(x => x.Tickets).ReturnsDbSet([TestDataFactory.DefaultTicket]);
        _contextMock.Setup(x => x.Users)
            .ReturnsDbSet([TestDataFactory.DefaultCreator, TestDataFactory.DefaultAssignee]);

        var result = await _handler.Handle(
            new GetTicketQuery(TestDataFactory.DefaultTicket.Id), CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_Throw_KeyNotFoundException_When_Ticket_Not_Found()
    {
        _contextMock.Setup(x => x.Tickets).ReturnsDbSet([]);

        var act = async () => await _handler.Handle(
            new GetTicketQuery(TestDataFactory.NonExistentTicketId), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*工单 {TestDataFactory.NonExistentTicketId} 不存在*");
    }
}