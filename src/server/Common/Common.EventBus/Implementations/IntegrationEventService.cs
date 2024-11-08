using System.Data.Common;
using Common.EventBus.Abstractions;
using Common.EventBus.Integrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.EventBus.Implementations;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly IEventBus _eventBus;
    private readonly DbContext _dbContext;
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly ILogger<IntegrationEventService> _logger;
    private readonly string _appName;

    public IntegrationEventService(Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory, IEventBus eventBus, DbContext dbContext, ILogger<IntegrationEventService> logger, string appName)
    {
        Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;

        _integrationEventLogServiceFactory = integrationEventLogServiceFactory;
        _eventBus = eventBus;
        _dbContext = dbContext;
        _eventLogService = _integrationEventLogServiceFactory(_dbContext.Database.GetDbConnection());
        _logger = logger;
        _appName = appName;
    }

    public async Task AddAndSaveEventAsync(IntegrationEvent integrationEvent)
    {
        _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", integrationEvent.Id, integrationEvent);

        await _eventLogService.SaveEventAsync(integrationEvent, _dbContext.Database.CurrentTransaction!);
    }

    public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
    {
        IEnumerable<IntegrationEventLog> pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

        foreach (IntegrationEventLog pendingLogEvent in pendingLogEvents)
        {
            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", pendingLogEvent.EventId, _appName, pendingLogEvent.IntegrationEvent);

            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(pendingLogEvent.EventId);
                await _eventBus.PublishAsync(pendingLogEvent.IntegrationEvent);
                await _eventLogService.MarkEventAsFailedAsync(pendingLogEvent.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", pendingLogEvent.EventId, _appName);

                await _eventLogService.MarkEventAsFailedAsync(pendingLogEvent.EventId);
            }
        }
    }
}