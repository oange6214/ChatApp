using ChatApp.Helpers;
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
    private string _lastSearchText;
    private string _lastSeen;
    private string _searchText;
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

    #region Search Chats

    public string LastSearchText
    {
        get => _lastSearchText;
        set => SetProperty(ref _lastSearchText, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                if (string.IsNullOrEmpty(value))
                {
                    Search();
                }
            }
        }
    }

    #endregion Search Chats

    #endregion Properties

    #region Commands

    private IRelayCommand _closeCommand;
    private IRelayCommand _maximizeCommand;
    private IRelayCommand _minimizeCommand;
    private IRelayCommand _searchCommand;
    public IRelayCommand CloseCommand => _closeCommand ??= new RelayCommand(Close);
    public IRelayCommand MaximizeCommand => _maximizeCommand ??= new RelayCommand(Maximize);
    public IRelayCommand MinimizeCommand => _minimizeCommand ??= new RelayCommand(Minimize);
    public IRelayCommand SearchCommand => _searchCommand ??= new RelayCommand(Search);

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

    #region Logics

    private void Search()
    {
        // To avoid re searching same text again
        if (string.IsNullOrEmpty(LastSearchText) && string.IsNullOrEmpty(SearchText) || string.Equals(LastSearchText, SearchText))
            return;

        // If searchbox is empty or chats is null pr chat cound less than 0
        if (string.IsNullOrEmpty(SearchText) || Chats == null || Chats.Count <= 0)
        {
            FilteredChats = new ObservableCollection<ChatListData>(Chats ?? Enumerable.Empty<ChatListData>());
            FilteredPinnedChats = new ObservableCollection<ChatListData>(PinnedChats ?? Enumerable.Empty<ChatListData>());

            // Update Last serach Text
            LastSearchText = SearchText;

            return;
        }

        // Now, to find the all chats that contain the text in our search box

        // if that chat is in Normal Unpinned Chat list find there...

        FilteredChats = new ObservableCollection<ChatListData>(
            Chats.Where(
                chat => chat.ContactName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) // if ContactName Contains SearchText then add it in filtered chat list
                ||
                chat.Message != null & chat.Message.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) // if Message Contains SearchText then add it in filtered chat list
                )
            );

        // else if not found in Normal Unpinned Chat list, find in pinned chats list
        FilteredPinnedChats = new ObservableCollection<ChatListData>(
            PinnedChats.Where(
                pinnedChat => pinnedChat.ContactName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) // if ContactName Contains SearchText then add it in filtered chat list
                ||
                pinnedChat.Message != null & pinnedChat.Message.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) // if Message Contains SearchText then add it in filtered chat list
                )
            );

        // Update Last serach Text
        LastSearchText = SearchText;
    }

    #endregion Logics

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
                    StatusImage = new Uri("/Assets/Images/8.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "Steve",
                    ContactPhoto = new Uri("/Assets/Images/2.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/7.png", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "Will",
                    ContactPhoto = new Uri("/Assets/Images/3.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/6.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "John",
                    ContactPhoto = new Uri("/Assets/Images/4.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/5.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
            ];
    }

    #endregion Logics

    #endregion Status Thumbs

    #region Chats List

    #region Fields

    private ObservableCollection<ChatListData> _chats = [];
    private ObservableCollection<ChatListData> _filteredChats = [];
    private ObservableCollection<ChatListData> _filteredPinnedChats = [];
    private ObservableCollection<ChatListData> _pinnedChats = [];

    #endregion Fields

    #region Properties

    public ObservableCollection<ChatListData> Chats
    {
        get => _chats;
        set
        {
            if (SetProperty(ref _chats, value))
            {
                // Updating filtered chats to match
                FilteredChats = new ObservableCollection<ChatListData>(_chats);
            }
        }
    }

    // Filtering Chats & Pinned Chats
    public ObservableCollection<ChatListData> FilteredChats
    {
        get => _filteredChats;
        set => SetProperty(ref _filteredChats, value);
    }

    public ObservableCollection<ChatListData> FilteredPinnedChats
    {
        get => _filteredPinnedChats;
        set => SetProperty(ref _filteredPinnedChats, value);
    }

    public ObservableCollection<ChatListData> PinnedChats
    {
        get => _pinnedChats;
        set
        {
            if (SetProperty(ref _pinnedChats, value))
            {
                // Updating filtered chats to match
                FilteredPinnedChats = new ObservableCollection<ChatListData>(_pinnedChats);
            }
        }
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
    private string _messageText;

    #endregion Fields

    #region Properties

    public ObservableCollection<ChatConversation> Conversations
    {
        get => _conversations;
        set => SetProperty(ref _conversations, value);
    }

    public string MessageText
    {
        get => _messageText;
        set => SetProperty(ref _messageText, value);
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

    // To get the ContactName of selected chat so that we can open corresponding conversation

    private IRelayCommand _getSelectedChatCommand;

    // To Pin Chat on Pin Button Click
    private IRelayCommand _pinChatCommand;

    // To Pin Chat on Pin Button Click
    private IRelayCommand _unPinChatCommand;

    public IRelayCommand GetSelectedChatCommand => _getSelectedChatCommand ?? new RelayCommand<ChatListData>(data =>
            {
                if (data == null)
                    return;

                //Getting ContactName from selected chat
                ContactName = data.ContactName;

                //Getting ContactPhoto from selected chat
                ContactPhoto = data.ContactPhoto;
            });

    public IRelayCommand PinChatCommand => _pinChatCommand ?? new RelayCommand<ChatListData>(data =>
    {
        if (data == null)
            return;

        if (!FilteredPinnedChats.Contains(data))
        {
            // Add selected chat to pin chat
            PinnedChats.Add(data);
            FilteredPinnedChats.Add(data);
            data.ChatIsPinned = true;

            // Remove selected chat from all chats / unpinned chats
            Chats.Remove(data);
            FilteredChats.Remove(data);
        }
    });

    public IRelayCommand UnPinChatCommand => _unPinChatCommand ?? new RelayCommand<ChatListData>(data =>
    {
        if (data == null)
            return;

        if (!FilteredChats.Contains(data))
        {
            // Add selected chat to Normal Unpinned chat list
            Chats.Add(data);
            FilteredChats.Add(data);

            // Remove selected pinned chats list
            PinnedChats.Remove(data);
            FilteredPinnedChats.Remove(data);
            data.ChatIsPinned = false;
        }
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