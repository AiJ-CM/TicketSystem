using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSystem.Application.Tickets.Queries.GetTicketList;

public class GetTicketListQueryValidator : AbstractValidator<GetTicketListQuery>
{
    public GetTicketListQueryValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1).WithMessage("页码不能小于1");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("每页条数必须在 1 到 100 之间");
    }
}
