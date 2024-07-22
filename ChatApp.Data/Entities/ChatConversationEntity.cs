namespace ChatApp.Data.Entities;

public class ChatConversationEntity
{
    public string ContactName { get; set; }
    public string LastOnline { get; set; }
    public string ReceivedMsgs { get; set; }
    public string MsgReceivedOn { get; set; }
    public bool IsReplied { get; set; }
    public bool IsRead { get; set; }
    public string SentMsgs { get; set; }
    public string MsgSentOn { get; set; }
    public string DocumentsReceived { get; set; }
    public string DocumentsSent { get; set; }
}