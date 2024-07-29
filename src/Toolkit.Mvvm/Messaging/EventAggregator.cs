using Toolkit.Mvvm.Messaging.Interfaces;

namespace Toolkit.Mvvm.Messaging;

public class EventAggregator : IEventAggregator
{
    private readonly Dictionary<Type, List<object>> _subscriptions = [];
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

    public void Publish<TEvent>(TEvent eventToPublish) where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(eventToPublish);

        var eventType = typeof(TEvent);

        if (_subscriptions.TryGetValue(eventType, out List<object>? value))
        {
            var subscriptions = value.ToList();

            foreach (var subscription in subscriptions)
            {
                var action = (Action<TEvent>)subscription;
                if (_synchronizationContext != null)
                {
                    _synchronizationContext.Post(_ => action(eventToPublish), null);
                }
                else
                {
                    action(eventToPublish);
                }
            }
        }
    }

    public void Subscribe<TEvent>(Action<TEvent> action) where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (!_subscriptions.TryGetValue(eventType, out List<object>? value))
        {
            value = ([]);
            _subscriptions[eventType] = value;
        }
        value.Add(action);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (_subscriptions.TryGetValue(eventType, out List<object>? value))
        {
            value.Remove(action);
        }
    }

    public void CleanupDeadSubscriptions()
    {
        _subscriptions.Clear();
    }
}