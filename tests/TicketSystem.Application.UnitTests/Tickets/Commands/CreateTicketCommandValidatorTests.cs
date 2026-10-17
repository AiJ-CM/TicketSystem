using FluentValidation.TestHelper;
using TicketSystem.Application.Tickets.Commands.CreateTicket;
using TicketSystem.Application.UnitTests.Fixtures;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class CreateTicketCommandValidatorTests
{
    private readonly CreateTicketCommandValidator _validator = new();

    [Fact]
    public void Title_Should_Not_Be_Empty()
    {
        var command = TestDataFactory.CreateCommand() with { Title = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Title_Should_Not_Exceed_MaxLength()
    {
        var command = TestDataFactory.CreateCommand() with { Title = new string('a', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Description_Should_Not_Be_Empty()
    {
        var command = TestDataFactory.CreateCommand() with { Description = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void TicketType_Should_Not_Be_Empty()
    {
        var command = TestDataFactory.CreateCommand() with { TicketType = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TicketType);
    }

    [Fact]
    public void CreatorId_Should_Be_Greater_Than_Zero()
    {
        var command = TestDataFactory.CreateCommand() with { CreatorId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreatorId);
    }

    [Fact]
    public void Should_Not_Have_Error_For_Valid_Command()
    {
        var command = TestDataFactory.CreateCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.TicketType);
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatorId);
    }
}