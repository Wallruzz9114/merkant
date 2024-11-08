using Common.WebApi.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.WebApi.Notifications;

public class ResultOkNotificationsFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        CancellationToken cancellationToken = context.HttpContext.RequestAborted;
        cancellationToken.ThrowIfCancellationRequested();

        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is ResultNotifications result && result.GetType().IsGenericType && result.IsValid)
            {
                object? data = result.GetType().GetProperty(nameof(Result<object>.Data))?.GetValue(result, null);
                context.Result = new OkObjectResult(data);
            }
        }

        base.OnResultExecuting(context);
    }
}