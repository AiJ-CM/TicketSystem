using FluentAssertions;
using FluentValidation.TestHelper;
using TicketSystem.Application.Tickets.Commands.CloseTicket;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class CloseTicketCommandValidatorTests
{
    private readonly CloseTicketCommandValidator _validator = new();

    [Fact]
    public void Positive_Id_Should_Pass() => _validator.TestValidate(new CloseTicketCommand(1)).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void NonPositive_Id_Should_Fail() => _validator.TestValidate(new CloseTicketCommand(0)).ShouldHaveValidationErrorFor(x => x.TicketId).WithErrorMessage("工单ID必须大于0");
}
