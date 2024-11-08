using Common.WebApi.Notifications;
using Common.WebApi.Postgres;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.WebApi.Shared;

public class DispatchDomainEventsResultNotificationPipeline<TRequest, TResponse>(ILogger<DispatchDomainEventsResultNotificationPipeline<TRequest, TResponse>> logger, IMediator mediator, IUnitOfWork unitOfWork, INotificationsContext notificationsContext) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultNotifications
{
    private readonly ILogger<DispatchDomainEventsResultNotificationPipeline<TRequest, TResponse>> _logger = logger;
    private readonly IMediator _mediator = mediator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INotificationsContext _notificationsContext = notificationsContext;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        TResponse response = await next();
        IEnumerable<Entity> entities = _unitOfWork.GetEntitiesPersistenceContext();

        foreach (Entity entity in entities)
        {
            _logger.LogInformation("----- Dispatch entity domain events result notifications {entityTypeName}", entity.GetType().Name);

            if (entity.HasDomainEvents)
            {
                List<INotification> domainEvents = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();

                foreach (INotification? domainEvent in domainEvents)
                {
                    if (_notificationsContext.HasErrors)
                        return (TResponse)_notificationsContext.Notifications;

                    _logger.LogInformation("----- Dispatch domain event result notifications {domainEvent}", domainEvent);
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }
        }

        return response;
    }
}