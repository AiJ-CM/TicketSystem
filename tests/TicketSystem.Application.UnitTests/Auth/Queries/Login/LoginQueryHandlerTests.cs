using TicketSystem.Application.Auth.Queries.Login;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using static BCrypt.Net.BCrypt;

namespace TicketSystem.Application.UnitTests.Auth.Queries.Login;

public class LoginQueryHandlerTests
{
    private readonly Mock<ITicketingDbContext> _db = new();
    private readonly Mock<ITokenService> _tokens = new();

    [Fact]
    public async Task Handle_Should_Return_LoginResult_For_Valid_Credentials()
    {
        var fixture = new Fixture();
        var user = fixture.Build<User>()
            .With(x => x.Id, 7)
            .With(x => x.UserName, "alice")
            .With(x => x.PasswordHash, HashPassword("Password9"))
            .With(x => x.IsActive, true)
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        _db.Setup(x => x.Users).ReturnsDbSet([user]);
        _tokens.Setup(x => x.CreateToken(user)).Returns("signed-token");
        var handler = new LoginQueryHandler(_db.Object, _tokens.Object);

        var result = await handler.Handle(new LoginQuery("alice", "Password9"), CancellationToken.None);

        result.Should().BeEquivalentTo(new LoginResult(7, "alice", "signed-token"));
        _tokens.Verify(x => x.CreateToken(user), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Reject_Unknown_User_Without_Issuing_Token()
    {
        _db.Setup(x => x.Users).ReturnsDbSet(Array.Empty<User>());
        var handler = new LoginQueryHandler(_db.Object, _tokens.Object);

        var act = async () => await handler.Handle(new LoginQuery("unknown", "Password9"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Invalid username or password")
            .Where(x => x.ErrorCode == ErrorCodeEnum.LoginFailed.ToString());
        _tokens.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Reject_Inactive_User_Without_Issuing_Token()
    {
        var fixture = new Fixture();
        var user = fixture.Build<User>()
            .With(x => x.Id, 8)
            .With(x => x.UserName, "inactive")
            .With(x => x.PasswordHash, HashPassword("Password9"))
            .With(x => x.IsActive, false)
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        _db.Setup(x => x.Users).ReturnsDbSet([user]);
        var handler = new LoginQueryHandler(_db.Object, _tokens.Object);

        var act = async () => await handler.Handle(new LoginQuery("inactive", "Password9"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>()
            .Where(x => x.ErrorCode == ErrorCodeEnum.LoginFailed.ToString());
        _tokens.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Reject_Wrong_Password_Without_Issuing_Token()
    {
        var fixture = new Fixture();
        var user = fixture.Build<User>()
            .With(x => x.Id, 9)
            .With(x => x.UserName, "alice")
            .With(x => x.PasswordHash, HashPassword("Correct9"))
            .With(x => x.IsActive, true)
            .Without(x => x.CreatedTickets)
            .Without(x => x.AssignedTickets)
            .Without(x => x.Audits)
            .Without(x => x.Comments)
            .Without(x => x.Roles)
            .Create();
        _db.Setup(x => x.Users).ReturnsDbSet([user]);
        var handler = new LoginQueryHandler(_db.Object, _tokens.Object);

        var act = async () => await handler.Handle(new LoginQuery("alice", "Wrong123"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>()
            .Where(x => x.ErrorCode == ErrorCodeEnum.LoginFailed.ToString());
        _tokens.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
    }
}