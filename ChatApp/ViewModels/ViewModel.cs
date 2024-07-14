﻿using ChatApp.Helpers;
using ChatApp.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using Toolkit.Wpf.Mvvm.ComponentModel;
using Toolkit.Wpf.Mvvm.Input;

namespace ChatApp.ViewModels;

public class ViewModel : ObservableObject
{
    #region MainWindow

    #region Fields

    private string _contactName;
    private Uri _contactPhoto;
    private string _lastSeen;
    private WindowState _windowState;

    #endregion Fields

    #region Properties

    public string ContactName
    {
        get => _contactName;
        set => SetProperty(ref _contactName, value);
    }

    public Uri ContactPhoto
    {
        get => _contactPhoto;
        set => SetProperty(ref _contactPhoto, value);
    }

    public string LastSeen
    {
        get => _lastSeen;
        set => SetProperty(ref _lastSeen, value);
    }

    public WindowState WindowState
    {
        get => _windowState;
        set => SetProperty(ref _windowState, value);
    }

    #endregion Properties

    #region Commands

    public IRelayCommand CloseCommand => new RelayCommand(Close);

    public IRelayCommand MaximizeCommand => new RelayCommand(Maximize);

    public IRelayCommand MinimizeCommand => new RelayCommand(Minimize);

    private void Close()
    {
        Application.Current.Shutdown();
    }

    private void Maximize()
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
        }
        else
        {
            WindowState = WindowState.Normal;
        }
    }

    private void Minimize()
    {
        WindowState = WindowState.Minimized;
    }

    #endregion Commands

    #endregion MainWindow

    #region Status Thumbs

    #region Fields

    private ObservableCollection<StatusDataModel> _statuses;

    #endregion Fields

    #region Properties

    public ObservableCollection<StatusDataModel> StatusThumbsCollection
    {
        get => _statuses;
        set => SetProperty(ref _statuses, value);
    }

    #endregion Properties

    #region Logics

    private void LoadStatusThumbs()
    {
        StatusThumbsCollection =
            [
                new StatusDataModel
                {
                    IsMeAddStatus = true,
                },
                new StatusDataModel
                {
                    ContactName = "Mike",
                    ContactPhoto = new Uri("/Assets/Images/1.png", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/5.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "Steve",
                    ContactPhoto = new Uri("/Assets/Images/2.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/8.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "Will",
                    ContactPhoto = new Uri("/Assets/Images/3.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/5.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "John",
                    ContactPhoto = new Uri("/Assets/Images/4.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/3.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
            ];
    }

    #endregion Logics

    #endregion Status Thumbs

    #region Chats List

    #region Fields

    private ObservableCollection<ChatListData> _chats;

    #endregion Fields

    #region Properties

    public ObservableCollection<ChatListData> Chats
    {
        get => _chats;
        set => SetProperty(ref _chats, value);
    }

    #endregion Properties

    #region Logics

    private void LoadChats()
    {
        Chats =
            [
                new ChatListData()
                {
                    ContactName = "Billy",
                    ContactPhoto = new Uri("/Assets/Images/6.jpg", UriKind.RelativeOrAbsolute),
                    Message = "Hey, What's up?",
                    LastMessageTime = "Tue, 12:58 PM",
                    ChatIsSelected = true
                },
                new ChatListData()
                {
                    ContactName = "Mike",
                    ContactPhoto = new Uri("/Assets/Images/1.png", UriKind.RelativeOrAbsolute),
                    Message = "Check the mail.",
                    LastMessageTime = "Mon, 10:07 AM"
                },
                new ChatListData()
                {
                    ContactName = "Steve",
                    ContactPhoto = new Uri("/Assets/Images/7.png", UriKind.RelativeOrAbsolute),
                    Message = "Yes, we had fun.",
                    LastMessageTime = "Tue, 08:10 AM"
                },
                new ChatListData()
                {
                    ContactName = "John",
                    ContactPhoto = new Uri("/Assets/Images/8.jpg", UriKind.RelativeOrAbsolute),
                    Message = "What about you?",
                    LastMessageTime = "Tue, 01:00 PM"
                },
            ];
    }

    #endregion Logics

    #endregion Chats List

    #region Conversations

    #region Fields

    private ObservableCollection<ChatConversation> _conversations;

    #endregion Fields

    #region Properties

    public ObservableCollection<ChatConversation> Conversations
    {
        get => _conversations;
        set => SetProperty(ref _conversations, value);
    }

    #endregion Properties

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

    #region Commands

    //To get the ContactName of selected chat so that we can open corresponding conversation

    private IRelayCommand _getSelectedChatCommand;

    public IRelayCommand GetSelectedChatCommand => _getSelectedChatCommand ?? new RelayCommand<ChatListData>(data =>
    {
        if (data == null)
            return;

        //Getting ContactName from selected chat
        ContactName = data.ContactName;

        //Getting ContactPhoto from selected chat
        ContactPhoto = data.ContactPhoto;
    });

    #endregion Commands

    #endregion Conversations

    public ViewModel()
    {
        LoadStatusThumbs();
        LoadChats();

        Task.Factory.StartNew(LoadChatConversation);
    }
}