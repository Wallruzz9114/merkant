using Common.WebApi.Postgres;
using Common.WebApi.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.WebApi.Shared;

public class DispatchDomainEventsResultPipeline<TRequest, TResponse>(ILogger<DispatchDomainEventsResultPipeline<TRequest, TResponse>> logger, IMediator mediator, IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<DispatchDomainEventsResultPipeline<TRequest, TResponse>> _logger = logger;
    private readonly IMediator _mediator = mediator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        TResponse response = await next();
        IEnumerable<Entity> entities = _unitOfWork.GetEntitiesPersistenceContext();

        foreach (Entity entity in entities)
        {
            _logger.LogInformation("----- Dispatch entity domain events result: {entityTypeName}", entity.GetType().Name);

            if (entity.HasDomainEvents)
            {
                List<INotification> domainEvents = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();

                foreach (INotification? domainEvent in domainEvents)
                {
                    if (response is Result result && !result.IsValid)
                        return response;

                    _logger.LogInformation("----- Dispatch domain event result {domainEvent}", domainEvent);
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }
        }

        return response;
    }
}