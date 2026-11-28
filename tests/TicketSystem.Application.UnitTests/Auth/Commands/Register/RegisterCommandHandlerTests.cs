using TicketSystem.Application.Auth.Commands.Register;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using static BCrypt.Net.BCrypt;

namespace TicketSystem.Application.UnitTests.Auth.Commands.Register;

public class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Persist_New_User_With_Hashed_Password()
    {
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Users).ReturnsDbSet(Array.Empty<User>());
        db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        User? added = null;
        db.Setup(x => x.Users.Add(It.IsAny<User>())).Callback<User>(u => { added = u; u.Id = 55; });
        var handler = new RegisterCommandHandler(db.Object, TimeProvider.System);

        var id = await handler.Handle(new RegisterCommand("alice_1", "Password9", "alice@example.com"), CancellationToken.None);

        id.Should().Be(55);
        added.Should().NotBeNull();
        added!.UserName.Should().Be("alice_1");
        added.Email.Should().Be("alice@example.com");
        added.IsActive.Should().BeTrue();
        added.CreatedBy.Should().Be("alice_1");
        Verify("Password9", added.PasswordHash).Should().BeTrue();
        db.Verify(x => x.Users.Add(It.IsAny<User>()), Times.Once);
        db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Reject_Duplicate_Username_Without_Persisting()
    {
        var fixture = new Fixture();
        var existing = fixture.Build<User>()
            .With(x => x.Id, 1)
            .With(x => x.UserName, "alice")
            .With(x => x.Email, "other@example.com")
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Users).ReturnsDbSet([existing]);
        var handler = new RegisterCommandHandler(db.Object, TimeProvider.System);

        var act = async () => await handler.Handle(new RegisterCommand("alice", "Password9", "alice@example.com"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>()
            .Where(x => x.ErrorCode == ErrorCodeEnum.UserCodeDuplicate.ToString());
        db.Verify(x => x.Users.Add(It.IsAny<User>()), Times.Never);
        db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Reject_Duplicate_Email_Without_Persisting()
    {
        var fixture = new Fixture();
        var existing = fixture.Build<User>()
            .With(x => x.Id, 1)
            .With(x => x.UserName, "other")
            .With(x => x.Email, "alice@example.com")
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Users).ReturnsDbSet([existing]);
        var handler = new RegisterCommandHandler(db.Object, TimeProvider.System);

        var act = async () => await handler.Handle(new RegisterCommand("alice", "Password9", "alice@example.com"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>()
            .Where(x => x.ErrorCode == ErrorCodeEnum.EmailDuplicate.ToString());
        db.Verify(x => x.Users.Add(It.IsAny<User>()), Times.Never);
        db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Surface_SaveFailure()
    {
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Users).ReturnsDbSet(Array.Empty<User>());
        db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("database unavailable"));
        var handler = new RegisterCommandHandler(db.Object, TimeProvider.System);

        var act = async () => await handler.Handle(new RegisterCommand("alice", "Password9", "alice@example.com"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("database unavailable");
    }
}