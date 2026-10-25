using FluentValidation;

namespace TicketSystem.Application.Tickets.Queries.GetTicket;

public class GetTicketQueryValidator : AbstractValidator<GetTicketQuery>
{
    public GetTicketQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("工单ID必须大于0");
    }
}