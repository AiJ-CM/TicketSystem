
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Commands.CreateTicket;
using TicketSystem.Application.UnitTests.Fixtures;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class CreateTicketCommandHandlerTests
{
    private readonly Mock<ITicketingDbContext> _contextMock;
    private readonly CreateTicketCommandHandler _handler;

    public CreateTicketCommandHandlerTests()
    {
        _contextMock = new Mock<ITicketingDbContext>();
        _contextMock.Setup(x => x.Tickets).ReturnsDbSet([]);
        _handler = new CreateTicketCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Add_Ticket_And_Return_Id()
    {
        var command = TestDataFactory.CreateCommand();

        int nextId = 1;
        Ticket addedTicket = null!;
        _contextMock.Setup(x => x.Tickets.Add(It.IsAny<Ticket>()))
                    .Callback<Ticket>(ticket => 
                    { 
                        addedTicket = ticket;
                        addedTicket.Id = nextId++;
                    });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeGreaterThan(0L);
        _contextMock.Verify(x => x.Tickets.Add(It.IsAny<Ticket>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Map_Fields_Correctly()
    {
        Ticket added = null!;
        _contextMock.Setup(x => x.Tickets.Add(It.IsAny<Ticket>()))
            .Callback<Ticket>(t => added = t);

        var command = TestDataFactory.CreateCommand(
            title: "支付失败",
            description: "订单支付超时",
            ticketType: TicketType.Bug,
            priority: TicketPriority.Medium,
            creatorId: 42);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        added.Should().NotBeNull();
        added.Title.Should().Be("支付失败");
        added.Description.Should().Be("订单支付超时");
        added.TicketType.Should().Be(TicketType.Bug);
        added.Priority.Should().Be(TicketPriority.Medium);
        added.CreatorId.Should().Be(42);
    }
}