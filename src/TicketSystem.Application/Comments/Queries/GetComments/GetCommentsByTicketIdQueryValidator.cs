using FluentValidation;

namespace TicketSystem.Application.Comments.Queries.GetComments;

public class GetCommentsByTicketIdQueryValidator : AbstractValidator<GetCommentsByTicketIdQuery>
{
    public GetCommentsByTicketIdQueryValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("工单ID必须大于0");
    }
}