using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class ValidationFilter<T>(IValidator<T> validator) : IActionFilter
    where T : class
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue("dto", out var value) || value is not T model)
            return;

        var result = validator.Validate(model);

        if (!result.IsValid)
        {
            context.Result = new ObjectResult(result.ToDictionary())
            {
                StatusCode = (int)HttpStatusCode.UnprocessableEntity
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}