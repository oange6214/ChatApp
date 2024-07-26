namespace Toolkit.Wpf.Mvvm.Messaging;

public class MessageSentEvent<T>
{
    public T Message { get; }

    public MessageSentEvent(T message)
    {
        Message = message;
    }
}