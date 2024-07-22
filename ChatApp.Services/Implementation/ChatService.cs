using AutoMapper;
using ChatApp.Data.Repositories;
using ChatApp.Domain.Models;
using ChatApp.Services.Interfaces;

namespace ChatApp.Services.Implementation;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;

    public ChatService(IChatRepository chatRepository, IMapper mapper)
    {
        _mapper = mapper;
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

    public async Task<IEnumerable<ChatConversation>> GetConversationsByContactNameAsync(string contactName)
    {
        var conversations = await _chatRepository.GetConversationsByContactNameAsync(contactName);
        return _mapper.Map<IEnumerable<ChatConversation>>(conversations);
    }
}