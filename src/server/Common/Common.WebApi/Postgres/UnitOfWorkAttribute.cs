using Common.EventBus.Abstractions;
using Common.WebApi.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Common.WebApi.Postgres;

[AttributeUsage(AttributeTargets.All)]
public class UnitOfWorkAttribute(IUnitOfWork unitOfWork, ILogger<UnitOfWorkAttribute> logger, IIntegrationEventService integrationEventService) : Attribute, IAsyncActionFilter
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UnitOfWorkAttribute> _logger = logger;
    private readonly IIntegrationEventService _integrationEventService = integrationEventService;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        CancellationToken cancellationToken = context.HttpContext.RequestAborted;
        cancellationToken.ThrowIfCancellationRequested();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        using IDbContextTransaction transaction = _unitOfWork.GetTransaction<IDbContextTransaction>();

        _logger.LogInformation("Begin transaction {TransactionId}", transaction.TransactionId);

        ActionExecutedContext result = await next();

        if (result.Exception is not null)
            return;

        try
        {
            if (_unitOfWork.HasActiveTransaction)
            {
                if (result.Result is ObjectResult objectResult && objectResult.Value is Result resultValue && resultValue.IsValid)
                {
                    await _unitOfWork.CommitAsync(cancellationToken);
                    _logger.LogInformation("Commited transaction {TransactionId}", transaction.TransactionId);
                    await _integrationEventService.PublishEventsThroughEventBusAsync(transaction.TransactionId);
                }
                else
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    _logger.LogInformation("Rollbacked transaction {TransactionId}", transaction.TransactionId);
                }
            }
        }
        catch (OperationCanceledException)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogInformation("Cancellation requested");
        }
    }
}