using FluentValidation;

namespace TicketSystem.Application.Auth.Queries.GetUser;

public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("用户ID必须大于0");
    }
}