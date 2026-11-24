using FluentAssertions;
using FluentValidation.TestHelper;
using TicketSystem.Application.Comments.Commands.CreateComment;

namespace TicketSystem.Application.UnitTests.Comments.Commands.CreateComment;

public class CreateCommentCommandValidatorTests
{
    private readonly CreateCommentCommandValidator _validator = new();

    [Fact]
    public void Valid_Comment_Should_Pass()
    {
        var result = _validator.TestValidate(new CreateCommentCommand(10, "Useful comment", false));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void TicketId_Should_Be_Greater_Than_Zero()
    {
        var result = _validator.TestValidate(new CreateCommentCommand(0, "Useful comment", false));
        result.ShouldHaveValidationErrorFor(x => x.TicketId).WithErrorMessage("工单ID必须大于0");
    }

    [Fact]
    public void Content_Should_Not_Be_Empty()
    {
        var result = _validator.TestValidate(new CreateCommentCommand(10, "", false));
        result.ShouldHaveValidationErrorFor(x => x.Content).WithErrorMessage("评论内容不能为空");
    }

    [Fact]
    public void Content_Should_Not_Exceed_2000_Characters()
    {
        var result = _validator.TestValidate(new CreateCommentCommand(10, new string('x', 2001), false));
        result.ShouldHaveValidationErrorFor(x => x.Content).WithErrorMessage("评论内容不能超过2000字符");
    }
}