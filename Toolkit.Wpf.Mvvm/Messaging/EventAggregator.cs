using System.Collections.Concurrent;
using Toolkit.Wpf.Mvvm.Messaging.Interfaces;

namespace Toolkit.Wpf.Mvvm.Messaging;

public class EventAggregator : IEventAggregator
{
    private readonly ConcurrentDictionary<Type, List<WeakReference<Delegate>>> _subScriptions = [];
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

    public void Publish<TEvent>(TEvent eventToPublish) where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(eventToPublish);

        var eventType = typeof(TEvent);
        if (_subScriptions.TryGetValue(eventType, out var subScriptionsList))
        {
            foreach (var weakReference in subScriptionsList)
            {
                if (weakReference.TryGetTarget(out var target))
                {
                    var action = (Action<TEvent>)target;
                    try
                    {
                        if (_synchronizationContext != null)
                        {
                            _synchronizationContext.Post(_ => action(eventToPublish), null);
                        }
                        else
                        {
                            action(eventToPublish);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error publishing event: {ex.Message}");
                    }
                }
                else
                {
                    subScriptionsList.Remove(weakReference);
                }
            }
        }
    }

    public void Subscribe<TEvent>(Action<TEvent> action) where TEvent : class
    {
        var eventType = typeof(TEvent);
        var subscriptionsList = _subScriptions.GetOrAdd(eventType, _ => []);
        subscriptionsList.Add(new WeakReference<Delegate>(action));
    }

    public void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (_subScriptions.TryGetValue(eventType, out var subscriptionsList))
        {
            subscriptionsList.RemoveAll(weakReference =>
                !weakReference.TryGetTarget(out var target) || target == (Delegate)action);
        }
    }

    public void CleanupDeadSubscriptions()
    {
        foreach (var subscriptionsList in _subScriptions.Values)
        {
            subscriptionsList.RemoveAll(weakReference => !weakReference.TryGetTarget(out _));
        }
    }
}