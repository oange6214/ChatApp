namespace ChatApp.Models;

public class ChatConversation
{
    public string ContactName { get; set; }
    public string ReceivedMessage { get; set; }
    public string MsgReceivedOn { get; set; }
    public string SentMessage { get; set; }
    public string MsgSentOn { get; set; }
    public bool IsMessageReceived { get; set; }
    public bool MessageContainsReply { get; set; }
}