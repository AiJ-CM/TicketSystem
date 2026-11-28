using TicketSystem.Application.Auth.Queries.Login;

namespace TicketSystem.Application.UnitTests.Auth.Queries.Login;

public class LoginQueryTests
{
    [Fact]
    public void Constructor_Should_Retain_Credentials_Exactly()
    {
        var query = new LoginQuery("admin_user", "S3cret99");

        query.Username.Should().Be("admin_user");
        query.Password.Should().Be("S3cret99");
    }
}