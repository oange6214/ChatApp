using ChatApp.Data.Entities;

namespace ChatApp.Data.Repositories;

public interface IChatRepository
{
    Task<IEnumerable<ChatConversationEntity>> GetAllConversationsAsync();

    Task<ChatConversationEntity> GetConversationByIdAsync(int id);

    Task<IEnumerable<ChatConversationEntity>> GetConversationsByContactNameAsync(string contactName);

    Task<int> AddConversationAsync(ChatConversationEntity conversation);

    Task<bool> UpdateConversationAsync(ChatConversationEntity conversation);

    Task<bool> DeleteConversationAsync(int id);

    Task<IEnumerable<ChatConversationEntity>> GetRecentConversationsAsync(int count);

    Task<int> GetTotalMessageCountAsync();

    Task<DateTime?> GetLastMessageTimeAsync(string contactName);
}