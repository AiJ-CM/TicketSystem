using FluentAssertions;
using FluentValidation.TestHelper;
using TicketSystem.Application.Tickets.Commands.SubmitResolution;

namespace TicketSystem.Application.UnitTests.Tickets.Commands;

public class SubmitResolutionCommandValidatorTests
{
    private readonly SubmitResolutionCommandValidator _validator = new();

    [Fact]
    public void Positive_Id_With_Note_Should_Pass()
        => _validator.TestValidate(new SubmitResolutionCommand(1, "已解决")).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void Invalid_TicketId_Should_Fail_With_TicketId_Property()
        => _validator.TestValidate(new SubmitResolutionCommand(0, null)).ShouldHaveValidationErrorFor(x => x.TicketId).WithErrorMessage("工单ID必须大于0");
}
