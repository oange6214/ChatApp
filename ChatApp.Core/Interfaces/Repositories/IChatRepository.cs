using ChatApp.Core.Entities;

namespace ChatApp.Core.Interfaces;

public interface IChatRepository
{
    Task<IEnumerable<ChatConversationEntity>> GetConversationsByContactNameAsync(string contactName);
}