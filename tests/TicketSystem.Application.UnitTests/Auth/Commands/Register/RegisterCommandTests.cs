using FluentAssertions;
using TicketSystem.Application.Auth.Commands.Register;

namespace TicketSystem.Application.UnitTests.Auth.Commands.Register;

public class RegisterCommandTests
{
    [Fact]
    public void Constructor_Should_Retain_Registration_Inputs()
    {
        var command = new RegisterCommand("alice_1", "Password9", "alice@example.com");

        command.Username.Should().Be("alice_1");
        command.Password.Should().Be("Password9");
        command.Email.Should().Be("alice@example.com");
    }
}