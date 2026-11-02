using FluentValidation;

namespace TicketSystem.Application.Tickets.Commands.CloseTicket;

public class CloseTicketCommandValidator : AbstractValidator<CloseTicketCommand>
{
    public CloseTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("工单ID必须大于0");
    }
}