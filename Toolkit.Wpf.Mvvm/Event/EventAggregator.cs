using Toolkit.Wpf.Mvvm.Event.Interfaces;

namespace Toolkit.Wpf.Mvvm.Event;

public class EventAggregator : IEventAggregator
{
    private readonly Dictionary<Type, List<object>> _subScriptions = [];
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

    public void Publish<TEvent>(TEvent eventToPublish)
    {
        if (eventToPublish == null)
            throw new ArgumentNullException(nameof(eventToPublish));

        var eventType = typeof(TEvent);
        if (_subScriptions.ContainsKey(eventType))
        {
            var subscriptions = _subScriptions[eventType].ToList();
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

    public void Subscribe<TEvent>(Action<TEvent> action)
    {
        var eventType = typeof(TEvent);
        if (!_subScriptions.ContainsKey(eventType))
        {
            _subScriptions[eventType] = [];
        }
        _subScriptions[eventType].Add(action);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> action)
    {
        var eventType = typeof(TEvent);
        if (_subScriptions.ContainsKey(eventType))
        {
            _subScriptions[eventType].Remove(action);
        }
    }
}