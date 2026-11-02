using FluentValidation;

namespace TicketSystem.Application.Tickets.Commands.AssignTicket;

public class AssignTicketCommandValidator : AbstractValidator<AssignTicketCommand>
{
    public AssignTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("工单ID必须大于0");
        RuleFor(x => x.AssigneeId).GreaterThan(0).WithMessage("被分配人ID必须大于0");
    }
}