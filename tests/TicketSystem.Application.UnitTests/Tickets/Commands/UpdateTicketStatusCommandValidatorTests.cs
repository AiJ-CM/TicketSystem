using FluentValidation.TestHelper;
using TicketSystem.Application.Tickets.Commands.UpdateStatus;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class UpdateTicketStatusCommandValidatorTests
{
    private readonly UpdateTicketStatusCommandValidator _validator = new();

    [Fact]
    public void Valid_IdAndStatus_Should_Pass()
        => _validator.TestValidate(new UpdateTicketStatusCommand(1, TicketStatus.InProgress)).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void Invalid_IdAndStatus_Should_Report_Both_Properties()
    {
        var result = _validator.TestValidate(new UpdateTicketStatusCommand(0, (TicketStatus)999));

        result.ShouldHaveValidationErrorFor(x => x.TicketId).WithErrorMessage("工单ID必须大于0");
        result.ShouldHaveValidationErrorFor(x => x.NewStatus).WithErrorMessage("工单状态不合法");
    }
}
