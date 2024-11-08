using Common.EventBus.Abstractions;
using Common.EventBus.Integrations;

namespace Common.EventBus.Implementations;

public partial class EventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    public class SubscriptionInfo
    {
        public Type HandlerType { get; }

        private SubscriptionInfo(Type handlerType) => HandlerType = handlerType;

        public static SubscriptionInfo Typed(Type handlerType) =>
            new(handlerType);
    }

    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

    private readonly List<Type> _eventTypes;

    public EventBusSubscriptionsManager()
    {
        _handlers = [];
        _eventTypes = [];
    }

    public bool IsEmpty => _handlers is { Count: 0 };

    public void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        string eventName = GetEventKey<T>();
        AddSubscription(typeof(T), eventName);

        if (!_eventTypes.Contains(typeof(T)))
            _eventTypes.Add(typeof(T));
    }

    public void Clear() => throw new NotImplementedException();

    public string GetEventKey<T>() => throw new NotImplementedException();

    public Type? GetEventTypeByName(string eventName) => throw new NotImplementedException();

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent => throw new NotImplementedException();

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => throw new NotImplementedException();

    public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
    {
        string key = GetEventKey<T>();
        return HasSubscriptionsForEvent(key);
    }

    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public void RemoveSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        SubscriptionInfo? handlerToRemove = FindSubscriptionToRemove<T, TH>();

        if (handlerToRemove is not null)
        {
            string eventName = GetEventKey<T>();
            RemoveHandler(eventName, handlerToRemove);
        }
    }

    private void AddSubscription(Type handlerType, string eventName)
    {
        if (!HasSubscriptionsForEvent(eventName))
        {
            _handlers.Add(eventName, []);
        }

        if (_handlers[eventName].Exists(s => s.HandlerType == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
        }

        _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
    }

    private SubscriptionInfo? FindSubscriptionToRemove<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        string eventName = GetEventKey<T>();
        return GetSubscriptionInfoToRemove(eventName, typeof(TH));
    }

    private SubscriptionInfo? GetSubscriptionInfoToRemove(string eventName, Type handlerType)
    {
        if (!HasSubscriptionsForEvent(eventName))
            return null;

        return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
    }

    private void RemoveHandler(string eventName, SubscriptionInfo subscriptionInfo)
    {
        if (subscriptionInfo is not null)
        {
            _handlers[eventName].Remove(subscriptionInfo);

            if (_handlers[eventName].Count == 0)
            {
                _handlers.Remove(eventName);
                Type? evenType = _eventTypes.SingleOrDefault(e => e.Name == eventName);

                if (evenType is not null)
                    _eventTypes.Remove(evenType);
            }
        }
    }
}
