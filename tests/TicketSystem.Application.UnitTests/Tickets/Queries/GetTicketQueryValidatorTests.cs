using FluentValidation.TestHelper;
using TicketSystem.Application.Tickets.Queries.GetTicket;

namespace TicketSystem.Application.UnitTests.Tickets.Queries;

public class GetTicketQueryValidatorTests
{
    private readonly GetTicketQueryValidator _validator = new();

    [Fact]
    public void Positive_Id_Should_Pass() => _validator.TestValidate(new GetTicketQuery(1)).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void NonPositive_Id_Should_Fail_TicketId_Property()
        => _validator.TestValidate(new GetTicketQuery(0)).ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("工单ID必须大于0");
}
