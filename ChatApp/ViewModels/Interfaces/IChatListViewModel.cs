using ChatApp.Domain.Models;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModels.Interfaces;

public interface IChatListViewModel
{
    ObservableCollection<ChatListItem> Chats { get; set; }
    ObservableCollection<ChatListItem> FilteredChats { get; set; }
    ObservableCollection<ChatListItem> FilteredPinnedChats { get; set; }
    ObservableCollection<ChatListItem> PinnedChats { get; set; }
    ObservableCollection<ChatListItem> ArchivedChats { get; set; }
}