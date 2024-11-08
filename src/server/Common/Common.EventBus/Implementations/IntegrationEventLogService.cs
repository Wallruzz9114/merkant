using System.Data.Common;
using System.Reflection;
using Common.EventBus.Abstractions;
using Common.EventBus.Integrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.EventBus.Implementations;

public class IntegrationEventLogService : IIntegrationEventLogService
{
    private readonly IntegrationEventContext _integrationEventContext;
    private readonly List<Type> _eventTypes;

    public IntegrationEventLogService(DbConnection dbConnection)
    {
        DbConnection _dbConnection;

        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        _integrationEventContext = new IntegrationEventContext(
            new DbContextOptionsBuilder<IntegrationEventContext>().UseNpgsql(_dbConnection).Options);
        _eventTypes = Assembly.GetAssembly(typeof(IntegrationEvent))!
            .GetTypes()
            .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
            .ToList();
    }

    public Task MarkEventAsFailedAsync(string eventId) => UpdateEventStatus(eventId, EventState.PublishedFailed);

    public Task MarkEventAsInProgressAsync(string eventId) => UpdateEventStatus(eventId, EventState.InProgress);

    public Task MarkEventAsPublishedAsync(string eventId) => UpdateEventStatus(eventId, EventState.Published);

    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        string tId = transactionId.ToString();
        List<IntegrationEventLog> integrationEventLogs = await _integrationEventContext.IntegrationEventLogs
            .Where(e => e.TransactionId == tId && e.State == EventState.NotPublished).ToListAsync();

        if (integrationEventLogs.Count != 0)
        {
            return integrationEventLogs
                .OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)!));
        }

        return [];
    }

    public Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction) => throw new NotImplementedException();

    private Task<int> UpdateEventStatus(string eventId, EventState status)
    {
        IntegrationEventLog eventLogEntry = _integrationEventContext.IntegrationEventLogs
            .Single(iEvent => iEvent.EventId == eventId);
        eventLogEntry.UpdateStatus(status);

        _integrationEventContext.IntegrationEventLogs.Update(eventLogEntry);
        return _integrationEventContext.SaveChangesAsync();
    }
}