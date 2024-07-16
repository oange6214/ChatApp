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

namespace ChatApp.ViewModels;

public class ConversationViewModel : ObservableObject, IConversationViewModel
{
    #region Fields

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

    //public ConversationViewModel()
    //{ }

    public ConversationViewModel(IChatListViewModel chatListVM)
    {
        _chatListVM = chatListVM;

        Task.Factory.StartNew(LoadChatConversation);
    }

    #endregion Ctors

    #region Logics

    private async Task LoadChatConversation()
    {
        Conversations ??= [];

        string query = "SELECT * FROM conversations WHERE ContactName=@ContactName";

        SqlParameter[] parameters =
        {
            SqlHelper.CreateParameter("@ContactName", "Mike", SqlDbType.NVarChar),
        };

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