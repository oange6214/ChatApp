namespace Toolkit.Mvvm.Messaging.Interfaces;

/// <summary>
/// Defines the contract for an event aggregator that manages publication and subscription of events.
/// </summary>
public interface IEventAggregator
{
    /// <summary>
    /// Publishes an event to all subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventToPublish">The event instance to publish.</param>
    void Publish<TEvent>(TEvent eventToPublish) where TEvent : class;

    /// <summary>
    /// Subscribes to a specific type of event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to subscribe to.</typeparam>
    /// <param name="action">The action to be executed when the event is published.</param>
    void Subscribe<TEvent>(Action<TEvent> action) where TEvent : class;

    /// <summary>
    /// Unsubscribes from a specific type of event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to unsubscribe from.</typeparam>
    /// <param name="action">The action to be removed from the subscription list.</param>
    void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : class;

    /// <summary>
    /// Removes all dead (garbage collected) subscriptions from the aggregator.
    /// </summary>
    void CleanupDeadSubscriptions();
}