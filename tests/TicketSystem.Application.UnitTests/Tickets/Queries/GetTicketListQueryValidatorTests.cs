using FluentValidation.TestHelper;
using TicketSystem.Application.Tickets.Queries.GetTicketList;

namespace TicketSystem.Application.UnitTests.Tickets.Queries;

public class GetTicketListQueryValidatorTests
{
    private readonly GetTicketListQueryValidator _validator = new();

    [Fact]
    public void Valid_Paging_Should_Pass() => _validator.TestValidate(new GetTicketListQuery { PageIndex = 1, PageSize = 20 }).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void PageIndex_Below_One_Should_Fail() => _validator.TestValidate(new GetTicketListQuery { PageIndex = 0, PageSize = 20 }).ShouldHaveValidationErrorFor(x => x.PageIndex).WithErrorMessage("页码不能小于1");

    [Fact]
    public void PageSize_Outside_One_To_100_Should_Fail_For_Both_Boundaries()
    {
        var below = _validator.TestValidate(new GetTicketListQuery { PageSize = 0 });
        var above = _validator.TestValidate(new GetTicketListQuery { PageSize = 101 });

        below.ShouldHaveValidationErrorFor(x => x.PageSize).WithErrorMessage("每页条数必须在 1 到 100 之间");
        above.ShouldHaveValidationErrorFor(x => x.PageSize).WithErrorMessage("每页条数必须在 1 到 100 之间");
    }
}
