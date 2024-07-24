namespace ChatApp.EventArgs;

public class ReplyMessageEventArgs
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool FocusMessageBox { get; set; }
    public string MessageToReplyText { get; set; }
    public bool IsThisAReplyMessage { get; set; }
}