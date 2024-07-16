namespace Toolkit.Wpf.Mvvm.Event.Interfaces;

public interface IEventAggregator
{
    void Publish<TEvent>(TEvent eventToPublish);

    void Subscribe<TEvent>(Action<TEvent> action);

    void Unsubscribe<TEvent>(Action<TEvent> action);
}