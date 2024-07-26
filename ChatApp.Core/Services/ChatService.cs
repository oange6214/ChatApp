using AutoMapper;
using ChatApp.Core.Interfaces;
using ChatApp.Core.Models;

namespace ChatApp.Core.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;

    public ChatService(IChatRepository chatRepository, IMapper mapper)
    {
        _mapper = mapper;
        _chatRepository = chatRepository;
    }

    public Task<IEnumerable<ChatConversation>> GetContact()
    {
        throw new NotImplementedException();
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