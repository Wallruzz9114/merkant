using Common.EventBus.Integrations;

namespace Common.EventBus.Abstractions;

public interface IIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync(IntegrationEvent integrationEvent);
}