using ChatApp.Domain.Models;

namespace ChatApp.Services.Interfaces;

public interface IChatService
{
    //Task<bool> AddConversationAsync(ChatConversationDto conversation);

    //Task<IEnumerable<ChatConversationDto>> GetAllConversationsAsync();

    //Task<ChatConversationDto> GetConversationByIdAsync(int id);

    Task<IEnumerable<ChatConversationDto>> GetConversationsByContactNameAsync(string contactName);
}