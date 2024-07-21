using ChatApp.CustomControls;
using ChatApp.Events;
using ChatApp.Helpers;
using ChatApp.Models;
using ChatApp.ViewModels.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using Toolkit.Wpf.Mvvm.ComponentModel;
using Toolkit.Wpf.Mvvm.Input;
using Toolkit.Wpf.Mvvm.Messaging.Interfaces;

namespace ChatApp.ViewModels;

public class ConversationViewModel : ObservableObject, IConversationViewModel
{
    #region Fields

    private IChatListViewModel _chatListVM;
    private ObservableCollection<ChatConversation> _conversations;
    private IEventAggregator _eventAggregator;
    private ObservableCollection<ChatConversation> _filteredConversations;

    #endregion Fields

    #region Properties

    public ObservableCollection<ChatConversation> Conversations
    {
        get => _conversations;
        set
        {
            if (SetProperty(ref _conversations, value))
            {
                // Updating filtered chats to match
                FilteredConversations = new ObservableCollection<ChatConversation>(_conversations);
            }
        }
    }

    /// <summary>
    /// Filter Conversation
    /// </summary>
    public ObservableCollection<ChatConversation> FilteredConversations
    {
        get => _filteredConversations;
        set => SetProperty(ref _filteredConversations, value);
    }

    private string _lastSearchConversationText;
    private string _searchConversationText;

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

    #region Ctors

    public ConversationViewModel(
        IEventAggregator eventAggregator,
        IChatListViewModel chatListVM)
    {
        _eventAggregator = eventAggregator;
        _chatListVM = chatListVM;

        _eventAggregator.Subscribe<ChatListDataEvent>(OnChatListDataEvent);
    }

    private async void OnChatListDataEvent(ChatListDataEvent chatEvent)
    {
        await LoadChatConversation(chatEvent.Data);
    }

    #endregion Ctors

    #region Logics

    private async Task LoadChatConversation(ChatListData chat)
    {
        Conversations ??= [];

        Conversations.Clear();
        FilteredConversations.Clear();

        // SQL config
        string query = "SELECT * FROM conversations WHERE ContactName=@ContactName";

        SqlParameter[] parameters =
        {
            SqlHelper.CreateParameter("@ContactName", chat.ContactName, SqlDbType.NVarChar),
        };

        // Call SQL
        try
        {
            ObservableCollection<ChatConversation> temp = [];
            await SqlHelper.ExecuteReaderAsync(query, (args) =>
            {
                var data = args.Data as dynamic;

                ChatConversation conversation = new()
                {
                    ContactName = data.ContactName,
                    ReceivedMessage = data.ReceivedMessage,
                    MsgReceivedOn = data.MsgReceivedOn,
                    SentMessage = data.SentMessage,
                    MsgSentOn = data.MsgSentOn,
                    IsMessageReceived = data.IsMessageReceived
                };

                temp.Add(conversation);
            }, parameters);

            Conversations = temp;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading chat conversations: {ex.Message}");
        }
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
            FilteredConversations = new ObservableCollection<ChatConversation>(Conversations ?? Enumerable.Empty<ChatConversation>());

            // Update Last serach Text
            _lastSearchConversationText = SearchConversationText;
            return;
        }

        FilteredConversations = new ObservableCollection<ChatConversation>(
            Conversations.Where(chat =>
                chat.ReceivedMessage.Contains(SearchConversationText, StringComparison.CurrentCultureIgnoreCase)
                || chat.SentMessage.Contains(SearchConversationText, StringComparison.CurrentCultureIgnoreCase)
            )
        );

        // Update Last serach Text
        _lastSearchConversationText = SearchConversationText;
    }

    #endregion Logics

    #region Commands

    private IRelayCommand _searchConversationCommand;

    public IRelayCommand SearchConversationCommand => _searchConversationCommand ??= new RelayCommand(SearchConversation);

    #endregion Commands
}