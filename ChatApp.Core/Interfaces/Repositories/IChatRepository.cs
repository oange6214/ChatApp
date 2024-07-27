using ChatApp.Core.Entities;
using ChatApp.Core.Models;

namespace ChatApp.Core.Interfaces;

public interface IChatRepository
{
    Task<IEnumerable<Conversation>> GetConversationsByContactNameAsync(string contactName);

    Task<IEnumerable<(Contact, Conversation)>> GetContactsWithLatestConversationsAsync();
}