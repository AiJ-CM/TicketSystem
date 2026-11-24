using FluentAssertions;
using FluentValidation.TestHelper;
using TicketSystem.Application.Auth.Commands.Register;

namespace TicketSystem.Application.UnitTests.Auth.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Valid_Registration_Should_Pass()
    {
        var result = _validator.TestValidate(new RegisterCommand("alice_1", "Password9", "alice@example.com"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Username_Should_Be_Required()
    {
        var result = _validator.TestValidate(new RegisterCommand("", "Password9", "alice@example.com"));
        result.ShouldHaveValidationErrorFor(x => x.Username).WithErrorMessage("Username is required");
    }

    [Fact]
    public void Username_Should_Enforce_Minimum_Maximum_And_Format()
    {
        var shortResult = _validator.TestValidate(new RegisterCommand("ab", "Password9", "alice@example.com"));
        shortResult.ShouldHaveValidationErrorFor(x => x.Username).WithErrorMessage("Username must be at least 3 characters");

        var invalidFormat = _validator.TestValidate(new RegisterCommand("alice-name", "Password9", "alice@example.com"));
        invalidFormat.ShouldHaveValidationErrorFor(x => x.Username).WithErrorMessage("Username can only contain letters, numbers, and underscores");

        var longResult = _validator.TestValidate(new RegisterCommand(new string('a', 51), "Password9", "alice@example.com"));
        longResult.ShouldHaveValidationErrorFor(x => x.Username).WithErrorMessage("Username must be at most 50 characters");
    }

    [Fact]
    public void Password_Should_Require_Length_And_Letter_Number_Mix()
    {
        var shortResult = _validator.TestValidate(new RegisterCommand("alice", "Abc12345", "alice@example.com"));
        shortResult.IsValid.Should().BeTrue();

        var missingNumber = _validator.TestValidate(new RegisterCommand("alice", "Password", "alice@example.com"));
        missingNumber.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password must contain both letters and numbers");

        var tooShort = _validator.TestValidate(new RegisterCommand("alice", "A1b2", "alice@example.com"));
        tooShort.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password must be at least 8 characters");
    }

    [Fact]
    public void Email_Should_Require_Valid_Format_And_Maximum_Length()
    {
        var invalid = _validator.TestValidate(new RegisterCommand("alice", "Password9", "not-an-email"));
        invalid.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email address is invalid");

        var tooLong = _validator.TestValidate(new RegisterCommand("alice", "Password9", new string('a', 92) + "@example.com"));
        tooLong.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email must be at most 100 characters");
    }
}