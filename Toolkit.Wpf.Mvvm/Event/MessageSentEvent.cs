namespace Toolkit.Wpf.Mvvm.Event;

public class MessageSentEvent
{
    public string Message { get; }

    public MessageSentEvent(string message)
    {
        Message = message;
    }
}