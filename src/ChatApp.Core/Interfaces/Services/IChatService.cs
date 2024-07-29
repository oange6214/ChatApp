using ChatApp.Core.Models;
using System.Collections.ObjectModel;

namespace ChatApp.Core.Interfaces;

public interface IChatService
{
    Task<IEnumerable<ChatConversationDto>> GetConversationsByContactNameAsync(string contactName);

    Task<ObservableCollection<ChatListItemDto>> GetChatListAsync();
}