using ChatApp.Core.Entities;

namespace ChatApp.Core.Interfaces;

public interface IChatRepository
{
    Task<IEnumerable<Conversation>> GetConversationsByContactNameAsync(string contactName);

    Task<IEnumerable<(Contact, Conversation)>> GetContactsWithLatestConversationsAsync();
}