using ChatApp.Domain.Models;

namespace ChatApp.EventArgs;

public class ChatListDataEventArgs
{
    public string GUID { get; } = Guid.NewGuid().ToString();
    public ChatListData Data { get; set; }
}