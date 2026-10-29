using FluentValidation;

namespace TicketSystem.Application.Comments.Commands.CreateComment;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("工单ID必须大于0");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("评论内容不能为空")
            .MaximumLength(2000).WithMessage("评论内容不能超过2000字符");
    }
}