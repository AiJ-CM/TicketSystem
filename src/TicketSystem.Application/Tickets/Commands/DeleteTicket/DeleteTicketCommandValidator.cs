using FluentValidation;

namespace TicketSystem.Application.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandValidator : AbstractValidator<DeleteTicketCommand>
{
    public DeleteTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("工单ID必须大于0");
    }
}