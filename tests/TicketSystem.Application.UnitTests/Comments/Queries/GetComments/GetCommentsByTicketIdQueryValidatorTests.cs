using FluentAssertions;
using FluentValidation.TestHelper;
using TicketSystem.Application.Comments.Queries.GetComments;

namespace TicketSystem.Application.UnitTests.Comments.Queries.GetComments;

public class GetCommentsByTicketIdQueryValidatorTests
{
    private readonly GetCommentsByTicketIdQueryValidator _validator = new();

    [Fact]
    public void Positive_TicketId_Should_Pass()
    {
        var result = _validator.TestValidate(new GetCommentsByTicketIdQuery(1));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Zero_Or_Negative_TicketId_Should_Fail()
    {
        _validator.TestValidate(new GetCommentsByTicketIdQuery(0))
            .ShouldHaveValidationErrorFor(x => x.TicketId).WithErrorMessage("工单ID必须大于0");
        _validator.TestValidate(new GetCommentsByTicketIdQuery(-1))
            .ShouldHaveValidationErrorFor(x => x.TicketId).WithErrorMessage("工单ID必须大于0");
    }
}