using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Tickets.Commands.CreateTicket;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class CreateTicketCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Ticket_With_Current_User_And_Return_Id()
    {
        // Arrange
        const long currentUserId = 1001L;
        const long generatedTicketId = 27L;
        var database = new Mock<ITicketingDbContext>();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.UserId).Returns(currentUserId);
        database.Setup(context => context.Tickets).ReturnsDbSet([]);

        Ticket? addedTicket = null;
        database.Setup(context => context.Tickets.Add(It.IsAny<Ticket>()))
            .Callback<Ticket>(ticket =>
            {
                ticket.Id = generatedTicketId;
                addedTicket = ticket;
            });
        database.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateTicketCommandHandler(database.Object, currentUser.Object);
        var command = new CreateTicketCommand(
            "Payment failed",
            "Payment request timed out",
            TicketType.Bug,
            TicketPriority.High);
        using var cancellationSource = new CancellationTokenSource();

        // Act
        var result = await handler.Handle(command, cancellationSource.Token);

        // Assert
        result.Should().Be(generatedTicketId);
        addedTicket.Should().NotBeNull();
        addedTicket!.Title.Should().Be("Payment failed");
        addedTicket.Description.Should().Be("Payment request timed out");
        addedTicket.TicketType.Should().Be(TicketType.Bug);
        addedTicket.Priority.Should().Be(TicketPriority.High);
        addedTicket.Status.Should().Be(TicketStatus.Open);
        addedTicket.CreatorId.Should().Be(currentUserId);
        database.Verify(context => context.Tickets.Add(It.IsAny<Ticket>()), Times.Once);
        database.Verify(context => context.SaveChangesAsync(cancellationSource.Token), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Propagate_Save_Failure()
    {
        // Arrange
        var database = new Mock<ITicketingDbContext>();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.UserId).Returns(1001L);
        database.Setup(context => context.Tickets).ReturnsDbSet([]);
        database.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database unavailable"));

        var handler = new CreateTicketCommandHandler(database.Object, currentUser.Object);
        var command = new CreateTicketCommand(
            "Payment failed",
            "Payment request timed out",
            TicketType.Bug,
            TicketPriority.High);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database unavailable");
        database.Verify(context => context.Tickets.Add(It.IsAny<Ticket>()), Times.Once);
        database.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
