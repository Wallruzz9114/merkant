using Common.EventBus.Integrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.EventBus.Abstractions;

public interface IIntegrationEventLogService
{
    Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
    Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
    Task MarkEventAsPublishedAsync(string eventId);
    Task MarkEventAsInProgressAsync(string eventId);
    Task MarkEventAsFailedAsync(string eventId);
}