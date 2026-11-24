using FluentAssertions;
using FluentValidation.TestHelper;
using TicketSystem.Application.Tickets.Commands.CreateTicket;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class CreateTicketCommandValidatorTests
{
    private readonly CreateTicketCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_Reject_Empty_Title()
    {
        // Arrange
        var command = new CreateTicketCommand(
            string.Empty,
            "Valid description",
            TicketType.Bug,
            TicketPriority.Medium);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(ticket => ticket.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Validate_Should_Reject_Whitespace_Title()
    {
        // Arrange
        var command = new CreateTicketCommand(
            "   ",
            "Valid description",
            TicketType.Bug,
            TicketPriority.Medium);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(ticket => ticket.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Validate_Should_Accept_Title_At_Maximum_Length()
    {
        // Arrange
        var title = new string('t', 200);
        var command = new CreateTicketCommand(
            title,
            "Valid description",
            TicketType.Bug,
            TicketPriority.Medium);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(ticket => ticket.Title);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Reject_Title_Over_Maximum_Length()
    {
        // Arrange
        var command = new CreateTicketCommand(
            new string('t', 201),
            "Valid description",
            TicketType.Bug,
            TicketPriority.Medium);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(ticket => ticket.Title)
            .WithErrorMessage("Title must be at most 200 characters");
    }

    [Fact]
    public void Validate_Should_Reject_Empty_Description()
    {
        // Arrange
        var command = new CreateTicketCommand(
            "Valid title",
            string.Empty,
            TicketType.Bug,
            TicketPriority.Medium);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(ticket => ticket.Description)
            .WithErrorMessage("Description is required");
    }

    [Fact]
    public void Validate_Should_Accept_Description_At_Maximum_Length()
    {
        // Arrange
        var description = new string('d', 2000);
        var command = new CreateTicketCommand(
            "Valid title",
            description,
            TicketType.Bug,
            TicketPriority.Medium);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(ticket => ticket.Description);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Reject_Description_Over_Maximum_Length()
    {
        // Arrange
        var command = new CreateTicketCommand(
            "Valid title",
            new string('d', 2001),
            TicketType.Bug,
            TicketPriority.Medium);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(ticket => ticket.Description)
            .WithErrorMessage("Description must be at most 2000 characters");
    }

    [Fact]
    public void Validate_Should_Reject_Invalid_Ticket_Type()
    {
        // Arrange
        var command = new CreateTicketCommand(
            "Valid title",
            "Valid description",
            (TicketType)999,
            TicketPriority.Medium);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(ticket => ticket.TicketType)
            .WithErrorMessage("Invalid ticket type");
    }

    [Fact]
    public void Validate_Should_Reject_Invalid_Priority()
    {
        // Arrange
        var command = new CreateTicketCommand(
            "Valid title",
            "Valid description",
            TicketType.Bug,
            (TicketPriority)999);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(ticket => ticket.Priority)
            .WithErrorMessage("Invalid priority");
    }

    [Fact]
    public void Validate_Should_Accept_Command_With_Valid_Fields()
    {
        // Arrange
        var command = new CreateTicketCommand(
            "Payment failure",
            "The payment request timed out.",
            TicketType.Bug,
            TicketPriority.High);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}