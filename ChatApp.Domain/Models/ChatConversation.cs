namespace ChatApp.Domain.Models;

public class ChatConversation
{
    public string ContactName { get; set; } = string.Empty;
    public bool IsMessageReceived { get; set; }
    public bool MessageContainsReply { get; set; }
    public string MsgReceivedOn { get; set; } = string.Empty;
    public string MsgSentOn { get; set; } = string.Empty;
    public string ReceivedMessage { get; set; } = string.Empty;
    public string SentMessage { get; set; } = string.Empty;
}