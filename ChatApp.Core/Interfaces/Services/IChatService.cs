using ChatApp.Core.Models;

namespace ChatApp.Core.Interfaces;

public interface IChatService
{
    //Task<bool> AddConversationAsync(ChatConversation conversation);

    //Task<IEnumerable<ChatConversation>> GetAllConversationsAsync();

    //Task<ChatConversation> GetConversationByIdAsync(int id);

    Task<IEnumerable<ChatConversation>> GetConversationsByContactNameAsync(string contactName);

    Task<IEnumerable<ChatConversation>> GetContact();
}