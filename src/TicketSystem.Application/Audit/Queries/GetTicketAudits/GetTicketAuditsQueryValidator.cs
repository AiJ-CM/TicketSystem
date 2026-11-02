using FluentValidation;

namespace TicketSystem.Application.Audit.Queries.GetTicketAudits;

internal class GetTicketAuditsQueryValidator : AbstractValidator<GetTicketAuditsQuery>
{
    public GetTicketAuditsQueryValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("工单ID必须大于0");
    }
}