using ChatApp.Domain.Models;

namespace ChatApp.Services.Interfaces;

public interface IChatService
{
    //Task<bool> AddConversationAsync(ChatConversation conversation);

    //Task<IEnumerable<ChatConversation>> GetAllConversationsAsync();

    //Task<ChatConversation> GetConversationByIdAsync(int id);

    Task<IEnumerable<ChatConversation>> GetConversationsByContactNameAsync(string contactName);
}