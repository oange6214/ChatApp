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
using Toolkit.Wpf.Mvvm.Messaging.Interfaces;

namespace ChatApp.ViewModels;

public class ConversationViewModel : ObservableObject, IConversationViewModel
{
    #region Fields

    private IEventAggregator _eventAggregator;
    private ObservableCollection<ChatConversation> _conversations;
    private IChatListViewModel _chatListVM;

    #endregion Fields

    #region Properties

    public ObservableCollection<ChatConversation> Conversations
    {
        get => _conversations;
        set => SetProperty(ref _conversations, value);
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

        // SQL config
        string query = "SELECT * FROM conversations WHERE ContactName=@ContactName";

        SqlParameter[] parameters =
        {
            SqlHelper.CreateParameter("@ContactName", "Mike", SqlDbType.NVarChar),
        };

        // Call SQL
        try
        {
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

                Application.Current.Dispatcher.Invoke(() => Conversations.Add(conversation));
            }, parameters);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading chat conversations: {ex.Message}");
        }
    }

    #endregion Logics
}