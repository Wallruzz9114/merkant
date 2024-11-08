using System.Collections.Concurrent;
using System.Text.Json;
using Common.EventBus.Abstractions;
using Common.EventBus.Integrations;
using Common.EventBus.Utils;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;

namespace Common.EventBus.Implementations;

public class KafkaEventBus : IEventBus, IDisposable
{
    private readonly ILogger<KafkaEventBus> _logger;
    private readonly EventBusSettings _eventBusSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly AsyncPolicyWrap _policyWrap;
    private readonly IProducer<string, string> _producer;
    private readonly int _retryCount;
    private readonly ConsumerConfig _consumerConfig;
    private readonly ConcurrentDictionary<string, IConsumer<string, string>> _consumers = new();

    public KafkaEventBus(ILogger<KafkaEventBus> logger,
        EventBusSettings eventBusSettings,
        IServiceProvider serviceProvider,
        IEventBusSubscriptionsManager subsManager,
        int retryCount = 5)
    {
        _logger = logger;
        _eventBusSettings = eventBusSettings;
        _serviceProvider = serviceProvider;
        _subsManager = subsManager;
        _retryCount = retryCount;

        AsyncRetryPolicy retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        AsyncCircuitBreakerPolicy circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(3, TimeSpan.FromMinutes(1), OnBreak, OnReset, OnHalfOpen);

        _policyWrap = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

        ProducerConfig producerConfig = new()
        {
            BootstrapServers = _eventBusSettings.BootstrapServer,
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();

        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _eventBusSettings.BootstrapServer,
            GroupId = _eventBusSettings.Group,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
    }

    public async Task PublishAsync(IntegrationEvent @event)
    {
        AsyncRetryPolicy policy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, "----- WARNING: Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
            });

        string eventName = @event.GetType().Name;
        _logger.LogTrace("----- Creating Kafka producer to publish event: {EventId} ({EventName})", @event.Id, eventName);

        string message = JsonSerializer.Serialize(@event, @event.GetType());

        await policy.ExecuteAsync(async () =>
        {
            _logger.LogTrace("----- Publishing event to Kafka: {EventId}", @event.Id);

            await _producer.ProduceAsync(eventName, new Message<string, string>
            {
                Key = @event.Key ?? Guid.NewGuid().ToString(),
                Value = message
            });
        });
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        string eventName = _subsManager.GetEventKey<T>();

        _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());
        _subsManager.AddSubscription<T, TH>();
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T> => throw new NotImplementedException();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _producer?.Dispose();
        _consumers.Values.ToList().ForEach(_ => _?.Dispose());
    }

    private void OnBreak(Exception exception, TimeSpan timespan) =>
        _logger.LogError("----- Consumer Open (onBreak) - {@exception}", exception);

    private void OnReset() => _logger.LogWarning("----- Closed (onReset)");

    private void OnHalfOpen() => _logger.LogWarning("----- Half Open (onHalfOpen)");

    private async Task ProcessEvent(string eventName, string message)
    {
        _logger.LogTrace("----- Processing Kafka event: {EventName}", eventName);

        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            IEnumerable<EventBusSubscriptionsManager.SubscriptionInfo> subscriptions = _subsManager.GetHandlersForEvent(eventName);

            foreach (EventBusSubscriptionsManager.SubscriptionInfo subscription in subscriptions)
            {
                object? handler = scope.ServiceProvider.GetService(subscription.HandlerType);

                if (handler is null)
                    continue;

                Type? eventType = _subsManager.GetEventTypeByName(eventName);
                object? integrationEvent = JsonSerializer.Deserialize(message, eventType!);
                Type concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType!);

                await Task.Yield();
                await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new object[] { integrationEvent!, })!;
            }
        }
        else
        {
            _logger.LogWarning("----- No subscription for Kafka event: {EventName}", eventName);
        }
    }
}