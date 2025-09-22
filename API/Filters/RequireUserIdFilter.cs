using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class RequireUserIdFilter : IAsyncActionFilter
{
    private const string UserIdItemKey = "UserId";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var items = context.HttpContext.Items;

        if (items.TryGetValue(UserIdItemKey, out var value) && value is Guid)
        {
            await next();
            return;
        }

        context.Result = new UnauthorizedObjectResult("Необходима авторизация");
    }
}