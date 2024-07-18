using ChatApp.Data.Repositories;
using ChatApp.Domain.Models;
using ChatApp.Services.Interfaces;

namespace ChatApp.Services.Implementation;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    //public async Task<bool> AddConversationAsync(ChatConversationDto conversation)
    //{
    //    int result = await _chatRepository.AddConversationAsync(conversation);
    //    return result > 0;
    //}

    //public async Task<IEnumerable<ChatConversationDto>> GetAllConversationsAsync()
    //{
    //    return await _chatRepository.GetAllConversationsAsync();
    //}

    //public async Task<ChatConversationDto> GetConversationByIdAsync(int id)
    //{
    //    return await _chatRepository.GetConversationByIdAsync(id);
    //}

    public async Task<IEnumerable<ChatConversationDto>> GetConversationsByContactNameAsync(string contactName)
    {
        //return await _chatRepository.GetConversationsByContactNameAsync(contactName);

        var src = await _chatRepository.GetConversationsByContactNameAsync(contactName);

        var dto = new List<ChatConversationDto>();
        foreach (var conversation in src)
        {
            dto.Add(new ChatConversationDto
            {
                ContactName = conversation.ContactName,
                ReceivedMessage = conversation.ReceivedMsgs,
                MsgReceivedOn = conversation.MsgReceivedOn,
                SentMessage = conversation.MsgReceivedOn,
                MsgSentOn = conversation.MsgSentOn,
            });
        }

        return dto;
    }
}