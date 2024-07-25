using ChatApp.Domain.Models;

namespace ChatApp.EventArgs;

public class ChatListDataEventArgs
{
    public string GUID { get; } = Guid.NewGuid().ToString();
    public ChatListItem Data { get; set; }
}