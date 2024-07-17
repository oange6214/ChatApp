using ChatApp.Models;

namespace ChatApp.Events;

public class ChatListDataEvent
{
    public string GUID { get; } = Guid.NewGuid().ToString();
    public ChatListData Data { get; set; }
}