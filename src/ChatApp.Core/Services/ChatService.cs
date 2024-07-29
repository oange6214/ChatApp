using AutoMapper;
using ChatApp.Core.Interfaces;
using ChatApp.Core.Models;
using System.Collections.ObjectModel;

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

    public async Task<ObservableCollection<ChatListItemDto>> GetChatListAsync()
    {
        var contactsWithConversations = await _chatRepository.GetContactsWithLatestConversationsAsync();

        // To avoid duplication
        string lastItem = string.Empty;
        string newItem = string.Empty;

        ObservableCollection<ChatListItemDto> temp = [];

        foreach (var tuple in contactsWithConversations)
        {
            string lastMessageTime = string.Empty;
            string lastMessage = string.Empty;

            // If the last message is received from sender than update time and lastMessge variables...
            if (!string.IsNullOrEmpty(tuple.Item2.MsgReceivedOn))
            {
                lastMessageTime = Convert.ToDateTime(tuple.Item2.MsgReceivedOn).ToString("ddd hh:mm tt");
                lastMessage = tuple.Item2.ReceivedMsgs;
            }

            // Else if we have sent last message then update accordingly...
            if (!string.IsNullOrEmpty(tuple.Item2.MsgSentOn))
            {
                lastMessageTime = Convert.ToDateTime(tuple.Item2.MsgSentOn).ToString("ddd hh:mm tt");
                lastMessage = tuple.Item2.MsgSentOn;
            }

            // If the chat is new or we are starting new conversation which means there will be no previous sent or recived msgs in that case...
            // Show 'Start new conversation' message...
            if (string.IsNullOrEmpty(lastMessage))
            {
                lastMessage = "Start new conversation";
            }

            // Update data in model...
            ChatListItemDto chat = new()
            {
                ContactPhotoUri = tuple.Item1.Photo,
                ContactName = tuple.Item1.ContactName,
                LastMessage = lastMessage,
                LastMessageTime = lastMessageTime
            };

            // Update
            newItem = tuple.Item1.ContactName;

            // If last added chat contact is not same as new one then only add...
            if (lastItem != newItem)
            {
                temp.Add(chat);
            }

            lastItem = newItem;
        }

        return temp;
    }

    public async Task<IEnumerable<ChatConversationDto>> GetConversationsByContactNameAsync(string contactName)
    {
        var conversations = await _chatRepository.GetConversationsByContactNameAsync(contactName);
        return _mapper.Map<IEnumerable<ChatConversationDto>>(conversations);
    }
}