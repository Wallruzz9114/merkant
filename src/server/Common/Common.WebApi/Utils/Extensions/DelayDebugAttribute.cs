using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.WebApi.Utils.Extensions;

#if DEBUG
[AttributeUsage(AttributeTargets.All)]
public class DelayDebugAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        CancellationToken cancellationToken = context.HttpContext.RequestAborted;
        cancellationToken.ThrowIfCancellationRequested();

        await Task.Delay(1000);
        await next();
    }
}
#endif