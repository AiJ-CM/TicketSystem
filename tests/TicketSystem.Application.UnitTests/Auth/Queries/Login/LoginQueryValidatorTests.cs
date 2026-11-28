using FluentValidation.TestHelper;
using TicketSystem.Application.Auth.Queries.Login;

namespace TicketSystem.Application.UnitTests.Auth.Queries.Login;

public class LoginQueryValidatorTests
{
    private readonly LoginQueryValidator _validator = new();

    [Fact]
    public void Valid_Credentials_Should_Pass()
    {
        var result = _validator.TestValidate(new LoginQuery("valid_user", "password1"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Missing_Username_Should_Fail_With_Concrete_Error()
    {
        var result = _validator.TestValidate(new LoginQuery("", "password1"));
        result.ShouldHaveValidationErrorFor(x => x.Username).WithErrorMessage("Username is required");
    }

    [Fact]
    public void Missing_Password_Should_Fail_With_Concrete_Error()
    {
        var result = _validator.TestValidate(new LoginQuery("valid_user", ""));
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password is required");
    }

    [Fact]
    public void Username_And_Password_Should_Enforce_Configured_Lengths()
    {
        var tooLong = _validator.TestValidate(new LoginQuery(new string('u', 51), new string('p', 101)));
        tooLong.ShouldHaveValidationErrorFor(x => x.Username);
        tooLong.ShouldHaveValidationErrorFor(x => x.Password);

        var shortPassword = _validator.TestValidate(new LoginQuery("valid_user", "12345"));
        shortPassword.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 6 characters");
    }
}