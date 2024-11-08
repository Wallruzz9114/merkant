using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Common.WebApi.Results;

public class ValidationFilterAttribute : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            List<ErrorResult> errors = [];

            foreach (string modelStateKey in context.ModelState.Keys)
            {
                Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateEntry? value = context.ModelState[modelStateKey];

                foreach (ModelError error in value?.Errors!)
                {
                    errors.Add(new ErrorResult(null, modelStateKey, error.ErrorMessage));
                }
            }

            context.Result = new BadRequestObjectResult(new Result(errors));
        }
        else
        {
            await next();
        }
    }
}