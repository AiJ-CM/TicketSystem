using FluentValidation;

namespace TicketSystem.Application.Tickets.Commands.SubmitResolution;

public class SubmitResolutionCommandValidator : AbstractValidator<SubmitResolutionCommand>
{
    public SubmitResolutionCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("工单ID必须大于0");
    }
}