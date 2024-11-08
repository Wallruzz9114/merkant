using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Common.WebApi.Postgres;

[AttributeUsage(AttributeTargets.All)]
public class UnitOfWorkExceptionAttribute(IUnitOfWork unitOfWork, ILogger<UnitOfWorkExceptionAttribute> logger) : Attribute, IAsyncActionFilter
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ILogger<UnitOfWorkExceptionAttribute> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        CancellationToken cancellationToken = context.HttpContext.RequestAborted;
        cancellationToken.ThrowIfCancellationRequested();

        ActionExecutedContext result = await next();

        if (result.Exception is not null && _unitOfWork.HasActiveTransaction)
        {
            using IDbContextTransaction transaction = _unitOfWork.GetTransaction<IDbContextTransaction>();
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogInformation("Rollbacked exception {TranslationId}", transaction.TransactionId);
        }
    }
}