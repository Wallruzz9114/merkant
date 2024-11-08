using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.WebApi.Notifications;

public class ResultBadRequestNotificationsFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        CancellationToken cancellationToken = context.HttpContext.RequestAborted;
        cancellationToken.ThrowIfCancellationRequested();

        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is ResultNotifications result && result.HasErrors)
                context.Result = new BadRequestObjectResult(result);
        }

        base.OnResultExecuting(context);
    }
}