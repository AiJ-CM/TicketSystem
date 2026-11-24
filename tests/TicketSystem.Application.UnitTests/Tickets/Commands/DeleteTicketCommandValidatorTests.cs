using FluentValidation.TestHelper;
using TicketSystem.Application.Tickets.Commands.DeleteTicket;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class DeleteTicketCommandValidatorTests
{
    private readonly DeleteTicketCommandValidator _validator = new();

    [Fact]
    public void Positive_Id_Should_Pass() => _validator.TestValidate(new DeleteTicketCommand(1)).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void NonPositive_Id_Should_Fail() => _validator.TestValidate(new DeleteTicketCommand(-1)).ShouldHaveValidationErrorFor(x => x.TicketId).WithErrorMessage("工单ID必须大于0");
}
