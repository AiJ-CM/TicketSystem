using FluentValidation;

namespace TicketSystem.Application.Tickets.Commands.UpdateStatus;

public class UpdateTicketStatusCommandValidator : AbstractValidator<UpdateTicketStatusCommand>
{
    public UpdateTicketStatusCommandValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("工单ID必须大于0");
        RuleFor(x => x.CurrentUserId).GreaterThan(0).WithMessage("当前用户ID必须大于0");
        RuleFor(x => x.NewStatus).IsInEnum().WithMessage("工单状态不合法");
    }
}