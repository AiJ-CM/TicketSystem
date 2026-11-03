using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TicketSystem.Application.Common.Models;

namespace TicketSystem.Api.Filters;

public class ApiResultFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult && context.HttpContext.Response.StatusCode == 200)
        {
            var value = objectResult.Value;

            if (value is not ApiResult)
            {
                objectResult.Value = ApiResult<object>.Success(value);
                objectResult.StatusCode = 200;
            }
        }
    }
}