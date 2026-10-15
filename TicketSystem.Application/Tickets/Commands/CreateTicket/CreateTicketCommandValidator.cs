using FluentValidation;

namespace TicketSystem.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("标题不能为空")
            .MaximumLength(200).WithMessage("标题不能超过200个字符");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("描述不能为空")
            .MaximumLength(2000).WithMessage("描述不能超过2000个字符");

        RuleFor(x => x.TicketType)
            .NotEmpty().WithMessage("工单类型不能为空")
            .MaximumLength(50).WithMessage("工单类型不能超过50个字符");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("优先级无效");

        RuleFor(x => x.CreatorId)
            .GreaterThan(0).WithMessage("创建人ID无效");
    }
}