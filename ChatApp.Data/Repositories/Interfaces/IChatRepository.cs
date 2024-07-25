using ChatApp.Data.Entities;

namespace ChatApp.Data.Repositories;

public interface IChatRepository
{
    Task<IEnumerable<ChatConversationEntity>> GetConversationsByContactNameAsync(string contactName);
}