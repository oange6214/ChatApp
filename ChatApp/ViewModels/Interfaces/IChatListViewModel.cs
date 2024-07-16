using ChatApp.Models;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModels.Interfaces;

public interface IChatListViewModel
{
    ObservableCollection<ChatListData> Chats { get; set; }
    ObservableCollection<ChatListData> FilteredChats { get; set; }
    ObservableCollection<ChatListData> FilteredPinnedChats { get; set; }
    ObservableCollection<ChatListData> PinnedChats { get; set; }
}