using FluentValidation;

namespace TicketSystem.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must be at most 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must be at most 2000 characters");

        RuleFor(x => x.TicketType)
            .NotEmpty().WithMessage("Ticket type is required")
            .MaximumLength(50).WithMessage("Ticket type must be at most 50 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority");

        RuleFor(x => x.CreatorId)
            .GreaterThan(0).WithMessage("Invalid creator ID");
    }
}