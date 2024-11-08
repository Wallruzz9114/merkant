using Common.EventBus.Abstractions;
using Common.EventBus.Utils;
using Common.WebApi.Postgres;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Common.WebApi.Shared;

public class TransactionBehavior<TRequest, TResponse>(ILogger<TransactionBehavior<TRequest, TResponse>> logger, IUnitOfWork unitOfWork, DbContext context, IIntegrationEventService integrationEventService) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly DbContext _context = context;
    private readonly IIntegrationEventService _integrationEventService = integrationEventService;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse? response = default;
        string typeName = request.GetGenericTypeName();

        try
        {
            if (_unitOfWork.HasActiveTransaction)
                return await next();

            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;

                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                using IDbContextTransaction transaction = _unitOfWork.GetTransaction<IDbContextTransaction>();
                using (_logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                {
                    _logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                    response = await next();

                    _logger.LogInformation("Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                    await _unitOfWork.CommitAsync(cancellationToken);
                    transactionId = transaction.TransactionId;
                }

                await _integrationEventService.PublishEventsThroughEventBusAsync(transactionId);
            });

            return response!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Handling transaction for {CommandName} ({@Command})", typeName, request);
            throw;
        }
    }
}