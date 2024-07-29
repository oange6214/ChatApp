namespace ChatApp.Core.Models;

public class ChatConversationDto
{
    public string ContactName { get; set; } = string.Empty;
    public bool IsMessageReceived { get; set; }
    public bool MessageContainsReply { get; set; }
    public string MsgReceivedOn { get; set; } = string.Empty;
    public string MsgSentOn { get; set; } = string.Empty;
    public string ReceivedMessage { get; set; } = string.Empty;
    public string SentMessage { get; set; } = string.Empty;
}