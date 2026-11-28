using TicketSystem.Application.Auth.Queries.Login;

namespace TicketSystem.Application.UnitTests.Auth.Queries.Login;

public class LoginResultTests
{
    [Fact]
    public void Constructor_Should_Preserve_User_Profile_And_Token()
    {
        var result = new LoginResult(42, "alice", "signed-token");

        result.Id.Should().Be(42);
        result.Username.Should().Be("alice");
        result.Token.Should().Be("signed-token");
    }
}