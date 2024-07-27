using ChatApp.Core.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Toolkit.Mvvm.Input;

namespace ChatApp.ViewModels;

public partial class MainWindowViewModel
{
    #region Conversation

    #region Fields

    private ObservableCollection<ChatConversationDto> _conversations;
    private ObservableCollection<ChatConversationDto> _filteredConversations;

    #endregion Fields

    #region Properties

    private bool _isSearchConversationBoxOpen;

    private string _lastSearchConversationText;

    private string _searchConversationText;

    public ObservableCollection<ChatConversationDto> Conversations
    {
        get => _conversations;
        set
        {
            if (SetProperty(ref _conversations, value))
            {
                // Updating filtered chats to match
                FilteredConversations = new ObservableCollection<ChatConversationDto>(_conversations);
            }
        }
    }

    /// <summary>
    /// Filter Conversation
    /// </summary>
    public ObservableCollection<ChatConversationDto> FilteredConversations
    {
        get => _filteredConversations;
        set => SetProperty(ref _filteredConversations, value);
    }

    public bool IsSearchConversationBoxOpen
    {
        get => _isSearchConversationBoxOpen;
        set
        {
            if (SetProperty(ref _isSearchConversationBoxOpen, value))
            {
                if (_isSearchConversationBoxOpen == false)
                {
                    SearchConversationText = string.Empty;
                }
            }
        }
    }

    public string SearchConversationText
    {
        get => _searchConversationText;
        set
        {
            if (SetProperty(ref _searchConversationText, value))
            {
                if (string.IsNullOrEmpty(value))
                {
                    SearchConversation();
                }
            }
        }
    }

    #endregion Properties

    #region Commands

    private IRelayCommand _clearConversationSearchCommand;
    private IRelayCommand _relayCommand;
    private IRelayCommand _searchConversationCommand;
    public IRelayCommand ClearConversationSearchCommand => _clearConversationSearchCommand ??= new RelayCommand(ClearConversationSearchBox);

    public IRelayCommand RelayCommand => _relayCommand ??= new RelayCommand<ChatConversationDto>(data =>
    {
        if (data == null)
            return;

        // If replying sender's message, else if replying own message
        MessageToReplyText = data.IsMessageReceived ? data.ReceivedMessage : data.SentMessage;

        // Set focus on LastMessage box whne user clicks reply button
        FocusMessageBox = true;

        // Flag this message as reply message
        IsThisAReplyMessage = true;
    });

    public IRelayCommand SearchConversationCommand => _searchConversationCommand ??= new RelayCommand(SearchConversation);

    #endregion Commands

    #region Logics

    public void ClearConversationSearchBox()
    {
        if (!string.IsNullOrEmpty(SearchConversationText))
        {
            SearchConversationText = string.Empty;
        }
        else
        {
            CloseConversationSearchBox();
        }
    }

    public void CloseConversationSearchBox() => IsSearchConversationBoxOpen = false;

    private bool ContainsText(string source, string searchText)
    {
        return !string.IsNullOrEmpty(source)
            && source.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
    }

    private async Task LoadChatConversation(ChatListItemDto chat)
    {
        Conversations ??= [];

        Conversations.Clear();
        FilteredConversations.Clear();

        try
        {
            var conversations = await _chatService.GetConversationsByContactNameAsync(chat.ContactName);

            if (conversations.Any())
            {
                var conversation = conversations.LastOrDefault();
                chat.LastMessage = !string.IsNullOrEmpty(conversation.ReceivedMessage) ? conversation.ReceivedMessage : conversation.SentMessage;
            }

            Conversations = new ObservableCollection<ChatConversationDto>(conversations);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading chat conversations: {ex.Message}");
        }

        // Reset reply message text when the new chat is fetched.
        MessageToReplyText = string.Empty;
    }

    private bool MatchesSearch(ChatConversationDto chat, string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return true;

        return ContainsText(chat.ReceivedMessage, searchText)
            || ContainsText(chat.SentMessage, searchText)
            || ContainsText(chat.ContactName, searchText)
            || ContainsText(chat.MsgReceivedOn, searchText)
            || ContainsText(chat.MsgSentOn, searchText);
    }

    private void SearchConversation()
    {
        // To avoid re searching same text again
        if (string.IsNullOrWhiteSpace(_lastSearchConversationText)
            && string.IsNullOrWhiteSpace(SearchConversationText)
            || string.Equals(_lastSearchConversationText, SearchConversationText, StringComparison.OrdinalIgnoreCase))
            return;

        // If searchbox is empty or chats is null pr chat cound less than 0
        if (string.IsNullOrWhiteSpace(SearchConversationText) || Conversations == null || Conversations.Count <= 0)
        {
            FilteredConversations = new ObservableCollection<ChatConversationDto>(Conversations ?? Enumerable.Empty<ChatConversationDto>());

            // Update Last serach Text
            _lastSearchConversationText = SearchConversationText;
            return;
        }

        FilteredConversations = new ObservableCollection<ChatConversationDto>(
            Conversations.Where(chat => MatchesSearch(chat, SearchConversationText))
        );

        // Update Last serach Text
        _lastSearchConversationText = SearchConversationText;
    }

    #endregion Logics

    #endregion Conversation
}