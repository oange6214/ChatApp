namespace Toolkit.Wpf.Mvvm.Messaging;

public class MessageSentEvent
{
    public string Message { get; }

    public MessageSentEvent(string message)
    {
        Message = message;
    }
}