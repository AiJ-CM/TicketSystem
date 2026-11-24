using FluentAssertions;
using FluentValidation.TestHelper;
using TicketSystem.Application.Tickets.Commands.AssignTicket;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class AssignTicketCommandValidatorTests
{
    private readonly AssignTicketCommandValidator _validator = new();

    [Fact]
    public void Valid_Assignment_Should_Pass()
        => _validator.TestValidate(new AssignTicketCommand(1, 2)).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void Invalid_TicketOrAssignee_Should_Report_Concrete_Properties()
    {
        var result = _validator.TestValidate(new AssignTicketCommand(0, -1));

        result.ShouldHaveValidationErrorFor(x => x.TicketId).WithErrorMessage("工单ID必须大于0");
        result.ShouldHaveValidationErrorFor(x => x.AssigneeId).WithErrorMessage("被分配人ID必须大于0");
    }
}
