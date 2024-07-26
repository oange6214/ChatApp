using ChatApp.Domain.Models;
using ChatApp.EventArgs;
using ChatApp.Models;
using ChatApp.Services.Interfaces;
using ChatApp.ViewModels.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Toolkit.Mvvm.ComponentModel;
using Toolkit.Mvvm.Input;
using Toolkit.Mvvm.Messaging.Interfaces;

namespace ChatApp.ViewModels;

public class MainWindowViewModel : ObservableObject, IMainWindowViewModel
{
    #region Ctors

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(
        IEventAggregator eventAggregator,
        IChatService chatService)
    {
        _eventAggregator = eventAggregator;
        _chatService = chatService;

        LoadChats();
        LoadStatusThumbs();
    }

    #endregion Ctors

    #region Main Window

    #region Fields

    // Initializing resource dictionary file
    private readonly ResourceDictionary dictionary = Application.LoadComponent(new Uri("/ChatApp;component/Assets/Images/icons.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;

    private ObservableCollection<MoreOptionMenu> _attachmentOptionsMenuList;
    private string _contactName;
    private byte[] _contactPhotoUri;
    private IEventAggregator _eventAggregator;
    private bool _isSearchBoxOpen;
    private string _lastSearchText;
    private string _lastSeen;
    private string _messageText;
    private string _searchText;
    private ObservableCollection<MoreOptionMenu> _windowMoreOptionsMenuList;
    private WindowState _windowState;

    #endregion Fields

    #region Properties

    public ObservableCollection<MoreOptionMenu> AttachmentOptionsMenuList
    {
        get => _attachmentOptionsMenuList;
        set => SetProperty(ref _attachmentOptionsMenuList, value);
    }

    public string ContactName
    {
        get => _contactName;
        set => SetProperty(ref _contactName, value);
    }

    public byte[] ContactPhotoUri
    {
        get => _contactPhotoUri;
        set => SetProperty(ref _contactPhotoUri, value);
    }

    public bool IsSearchBoxOpen
    {
        get => _isSearchBoxOpen;
        set
        {
            if (SetProperty(ref _isSearchBoxOpen, value))
            {
                if (!_isSearchBoxOpen)
                {
                    SearchText = string.Empty;
                }
            }
        }
    }

    public string LastSeen
    {
        get => _lastSeen;
        set => SetProperty(ref _lastSeen, value);
    }

    public string MessageText
    {
        get => _messageText;
        set => SetProperty(ref _messageText, value);
    }

    public ObservableCollection<MoreOptionMenu> WindowMoreOptionsMenuList
    {
        get => _windowMoreOptionsMenuList;
        set => SetProperty(ref _windowMoreOptionsMenuList, value);
    }

    public WindowState WindowState
    {
        get => _windowState;
        set => SetProperty(ref _windowState, value);
    }

    #region Search Chats

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                if (string.IsNullOrEmpty(_searchText))
                {
                    Search();
                }
            }
        }
    }

    #endregion Search Chats

    #endregion Properties

    #region ContactInfo

    #region Properties

    private bool _focusMessageBox;
    private bool _isContactInfoOpen;
    private bool _isThisAReplyMessage;
    private string _messageToReplyText;

    public bool FocusMessageBox
    {
        get => _focusMessageBox;
        set => SetProperty(ref _focusMessageBox, value);
    }

    public bool IsContactInfoOpen
    {
        get => _isContactInfoOpen;
        set => SetProperty(ref _isContactInfoOpen, value);
    }

    public bool IsThisAReplyMessage
    {
        get => _isThisAReplyMessage;
        set => SetProperty(ref _isThisAReplyMessage, value);
    }

    public string MessageToReplyText
    {
        get => _messageToReplyText;
        set => SetProperty(ref _messageToReplyText, value);
    }

    #endregion Properties

    #region Logics

    public void CloseContactInfo() => IsContactInfoOpen = false;

    public void OpenContactInfo() => IsContactInfoOpen = true;

    #endregion Logics

    #region Commands

    private IRelayCommand _closeContactInfoCommand;
    private IRelayCommand _openContactInfoCommand;
    public IRelayCommand CloseContactInfoCommand => _closeContactInfoCommand ??= new RelayCommand(CloseContactInfo);
    public IRelayCommand OpenContactInfoCommand => _openContactInfoCommand ??= new RelayCommand(OpenContactInfo);

    #endregion Commands

    #endregion ContactInfo

    #region Event Aggregator Methods

    private void OnChatSelectedEvent(ChatSelectedEventArgs evt)
    {
        ContactName = evt.ContactName;
        ContactPhotoUri = evt.ContactPhotoUri;
    }

    private void OnReplyMessageEvent(ReplyMessageEventArgs args)
    {
        FocusMessageBox = args.FocusMessageBox;
        MessageToReplyText = args.MessageToReplyText;
        IsThisAReplyMessage = args.IsThisAReplyMessage;
    }

    #endregion Event Aggregator Methods

    #region Commands

    private IRelayCommand _attachmentOptionsCommand;
    private IRelayCommand _cancelReplyCommand;
    private IRelayCommand _clearSearchCommand;
    private IRelayCommand _closeCommand;
    private IRelayCommand _conversationScreenMoreOptionsMenuCommand;
    private IRelayCommand _maximizeCommand;
    private IRelayCommand _minimizeCommand;
    private IRelayCommand _openConversationSearchCommand;
    private IRelayCommand _openSearchCommand;
    private IRelayCommand _searchCommand;
    private IRelayCommand _sendMessageCommand;
    private IRelayCommand _windowsMoreOptionsCommand;
    public IRelayCommand AttachmentOptionsCommand => _attachmentOptionsCommand ??= new RelayCommand(AttachmentOptionsMenu);
    public IRelayCommand CancelReplyCommand => _cancelReplyCommand ??= new RelayCommand(CancelReply);
    public IRelayCommand ClearSearchCommand => _clearSearchCommand ??= new RelayCommand(ClearSearchBox);
    public IRelayCommand CloseCommand => _closeCommand ??= new RelayCommand(Close);
    public IRelayCommand ConversationScreenMoreOptionsMenuCommand => _conversationScreenMoreOptionsMenuCommand ??= new RelayCommand(ConversationScreenMoreOptionsMenu);
    public IRelayCommand MaximizeCommand => _maximizeCommand ??= new RelayCommand(Maximize);
    public IRelayCommand MinimizeCommand => _minimizeCommand ??= new RelayCommand(Minimize);
    public IRelayCommand OpenConversationSearchCommand => _openConversationSearchCommand ??= new RelayCommand(OpenConversationSearchBox);
    public IRelayCommand OpenSearchCommand => _openSearchCommand ??= new RelayCommand(OpenSearchBox);
    public IRelayCommand SearchCommand => _searchCommand ??= new RelayCommand(Search);
    public IRelayCommand SendMessageCommand => _sendMessageCommand ??= new RelayCommand(SendMessage);
    public IRelayCommand WindowsMoreOptionsCommand => _windowsMoreOptionsCommand ??= new RelayCommand(WindowMoreOptionsMenu);

    #endregion Commands

    #region Logics

    #region Window: More options Popup

    private void AttachmentOptionsMenu()
    {
        // To populate menu items for Attachment Menu options list...
        AttachmentOptionsMenuList =
        [
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["docs"],
                MenuText = "Docs",
                BorderStroke = "#3F3990",
                Fill = "#CFCEEC"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["camera"],
                MenuText = "Camera",
                BorderStroke = "#2C5A71",
                Fill = "#C5E7F8"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["gallery"],
                MenuText = "Gallery",
                BorderStroke = "#EA2140",
                Fill = "#F7D5AC"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["audio"],
                MenuText = "Audio",
                BorderStroke = "#E67E00",
                Fill = "#F7D5AC"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["location"],
                MenuText = "Location",
                BorderStroke = "#28C58F",
                Fill = "#E3F5EF"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["contact"],
                MenuText = "Contact",
                BorderStroke = "#0093E0",
                Fill = "#DDF1FB"
            },
        ];
    }

    private void ConversationScreenMoreOptionsMenu()
    {
        // To populate items for conversation screen options list...
        WindowMoreOptionsMenuList =
        [
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["allmedia"],
                MenuText = "All Media"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["wallpaper"],
                MenuText = "Change Wallpaper"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["report"],
                MenuText = "Report"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["block"],
                MenuText = "Block"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["clearchat"],
                MenuText = "Clear Chat"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["exportchat"],
                MenuText = "Export Chat"
            },
        ];
    }

    private void WindowMoreOptionsMenu()
    {
        WindowMoreOptionsMenuList =
        [
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["newgroup"],
                MenuText = "New Group"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["newbroadcast"],
                MenuText = "New Broadcast"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["starredmessages"],
                MenuText = "Starred Messages"
            },
            new MoreOptionMenu
            {
                Icon = (PathGeometry)dictionary["settings"],
                MenuText = "Settings"
            },
        ];
    }

    #endregion Window: More options Popup

    public void CancelReply()
    {
        IsThisAReplyMessage = false;

        // Reset Reply LastMessage Text
        MessageToReplyText = string.Empty;
    }

    public void ClearSearchBox()
    {
        if (!string.IsNullOrEmpty(SearchText))
        {
            SearchText = string.Empty;
        }
        else
        {
            CloseSearchBox();
        }
    }

    public void CloseSearchBox() => IsSearchBoxOpen = false;

    public void OpenConversationSearchBox() => IsSearchConversationBoxOpen = true;

    public void OpenSearchBox() => IsSearchBoxOpen = true;

    public void SendMessage()
    {
        // Send message only when the textbox is not empty..

        if (!string.IsNullOrEmpty(MessageText))
        {
            var conversation = new ChatConversation
            {
                ReceivedMessage = MessageToReplyText,
                SentMessage = MessageText,
                MsgSentOn = DateTime.Now.ToString("MMM dd, hh:mm tt"),
                MessageContainsReply = IsThisAReplyMessage
            };

            // Add message to conversation list.
            FilteredConversations.Add(conversation);
            Conversations.Add(conversation);

            UpdateChatAndMoveUp(Chats, conversation);
            UpdateChatAndMoveUp(PinnedChats, conversation);
            UpdateChatAndMoveUp(FilteredChats, conversation);
            UpdateChatAndMoveUp(FilteredPinnedChats, conversation);
            UpdateChatAndMoveUp(ArchivedChats, conversation);

            // Clear Mesage properties and textbox when message is sent.
            MessageText = string.Empty;
            MessageToReplyText = string.Empty;
            IsThisAReplyMessage = false;
        }
    }

    // Move the chat contact on top when new message is sent or received.
    protected void UpdateChatAndMoveUp(ObservableCollection<ChatListItem> chatList, ChatConversation conversation)
    {
        // Check if the message sent it to the selected contact or not...
        var chat = chatList.FirstOrDefault(chat => chat.ContactName == ContactName);

        // if found.. than..
        if (chat != null)
        {
            // Update Contact Chat Last LastMessage and LastMessage Time...
            chat.LastMessage = MessageText;
            chat.LastMessageTime = conversation.MsgSentOn;

            // Move Chat on top when new message is received/sent...
            chatList.Move(chatList.IndexOf(chat), 0);
        }
    }

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

    private void Search()
    {
        // To avoid re searching same text again
        if (string.IsNullOrWhiteSpace(_lastSearchText) && string.IsNullOrWhiteSpace(SearchText) || string.Equals(_lastSearchText, SearchText, StringComparison.OrdinalIgnoreCase))
            return;

        // If searchbox is empty or chats is null pr chat cound less than 0
        if (string.IsNullOrWhiteSpace(SearchText) || Chats == null || Chats.Count <= 0)
        {
            FilteredChats = new ObservableCollection<ChatListItem>(Chats ?? Enumerable.Empty<ChatListItem>());
            FilteredPinnedChats = new ObservableCollection<ChatListItem>(PinnedChats ?? Enumerable.Empty<ChatListItem>());

            // Update Last serach Text
            _lastSearchText = SearchText;
            return;
        }

        // Now, to find the all chats that contain the text in our search box, if that chat is in Normal Unpinned Chat list find there...

        Func<ChatListItem, bool> searchPredicate = chat =>
            chat.ContactName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)  // if ContactName Contains SearchText then add it in filtered chat list
            || (chat.LastMessage != null && chat.LastMessage.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)); // if LastMessage Contains SearchText then add it in filtered chat list

        FilteredChats = new ObservableCollection<ChatListItem>(Chats.Where(searchPredicate));

        // else if not found in Normal Unpinned Chat list, find in pinned chats list
        FilteredPinnedChats = new ObservableCollection<ChatListItem>(PinnedChats.Where(searchPredicate));

        // Update Last serach Text
        _lastSearchText = SearchText;
    }

    #endregion Logics

    #endregion Main Window

    #region Chat List

    #region Fields

    private ObservableCollection<ChatListItem> _archivedChats = [];
    private ObservableCollection<ChatListItem> _chats = [];
    private IChatService _chatService;
    private ObservableCollection<ChatListItem> _filteredChats = [];
    private ObservableCollection<ChatListItem> _filteredPinnedChats = [];
    private ObservableCollection<ChatListItem> _pinnedChats = [];

    #endregion Fields

    #region Properties

    private int _chatPosition;

    public ObservableCollection<ChatListItem> ArchivedChats
    {
        get => _archivedChats;
        set => SetProperty(ref _archivedChats, value);
    }

    public ObservableCollection<ChatListItem> Chats
    {
        get => _chats;
        set
        {
            if (SetProperty(ref _chats, value))
            {
                // Updating filtered chats to match
                FilteredChats = new ObservableCollection<ChatListItem>(_chats);
            }
        }
    }

    // Filtering Chats & Pinned Chats
    public ObservableCollection<ChatListItem> FilteredChats
    {
        get => _filteredChats;
        set => SetProperty(ref _filteredChats, value);
    }

    public ObservableCollection<ChatListItem> FilteredPinnedChats
    {
        get => _filteredPinnedChats;
        set => SetProperty(ref _filteredPinnedChats, value);
    }

    public ObservableCollection<ChatListItem> PinnedChats
    {
        get => _pinnedChats;
        set
        {
            if (SetProperty(ref _pinnedChats, value))
            {
                // Updating filtered chats to match
                FilteredPinnedChats = new ObservableCollection<ChatListItem>(_pinnedChats);
            }
        }
    }

    #endregion Properties

    #region Logics

    private void LoadChats()
    {
        // Loading data from Database
        Chats ??= [];

        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Workspaces\Devolops\WPFs\ChatApp\ChatApp\Database\Database1.mdf;Integrated Security=True";
        string sql = @"
                        SELECT *
                        FROM contacts p
                        LEFT JOIN
	                        (
		                        SELECT a.*, ROW_NUMBER()
			                        OVER(PARTITION BY a.contactname ORDER BY a.id DESC) AS seqnum
		                        FROM conversations a
	                        ) a
	                        ON a.ContactName = p.contactname AND a.seqnum = 1
                        ORDER BY a.Id DESC
                        ";

        ObservableCollection<ChatListItem> temp = [];

        using (SqlConnection connection = new(connectionString))
        {
            connection.Open();

            using SqlCommand commad = new(sql, connection);
            using SqlDataReader reader = commad.ExecuteReader();

            // To avoid duplication
            string lastItem = string.Empty;
            string newItem = string.Empty;

            while (reader.Read())
            {
                string lastMessageTime = string.Empty;
                string lastMessage = string.Empty;

                // If the last message is received from sender than update time and lastMessge variables...
                if (!string.IsNullOrEmpty(reader["MsgReceivedOn"].ToString()))
                {
                    lastMessageTime = Convert.ToDateTime(reader["MsgReceivedOn"].ToString()).ToString("ddd hh:mm tt");
                    lastMessage = reader["ReceivedMsgs"].ToString();
                }

                // Else if we have sent last message then update accordingly...
                if (!string.IsNullOrEmpty(reader["MsgSentOn"].ToString()))
                {
                    lastMessageTime = Convert.ToDateTime(reader["MsgSentOn"].ToString()).ToString("ddd hh:mm tt");
                    lastMessage = reader["SentMsgs"].ToString();
                }

                // If the chat is new or we are starting new conversation which means there will be no previous sent or recived msgs in that case...
                // Show 'Start new conversation' message...
                if (string.IsNullOrEmpty(lastMessage))
                {
                    lastMessage = "Start new conversation";
                }

                // Update data in model...
                ChatListItem chat = new()
                {
                    ContactPhotoUri = (byte[])reader["photo"],
                    ContactName = reader["contactname"].ToString(),
                    LastMessage = lastMessage,
                    LastMessageTime = lastMessageTime
                };

                // Update
                newItem = reader["contactname"].ToString();

                // If last added chat contact is not same as new one then only add...
                if (lastItem != newItem)
                {
                    temp.Add(chat);
                }

                lastItem = newItem;
            }
        }

        // Transfer data
        Chats = temp;
    }

    //Chats =
    //[
    //    new ChatListItem()
    //    {
    //        ContactName = "Billy",
    //        ContactPhotoUri = new Uri("/Assets/Images/6.jpg", UriKind.RelativeOrAbsolute),
    //        LastMessage = "Hey, What's up?",
    //        LastMessageTime = "Tue, 12:58 PM",
    //        IsSelected = true
    //    },
    //    new ChatListItem()
    //    {
    //        ContactName = "Mike",
    //        ContactPhotoUri = new Uri("/Assets/Images/1.png", UriKind.RelativeOrAbsolute),
    //        LastMessage = "Check the mail.",
    //        LastMessageTime = "Mon, 10:07 AM"
    //    },
    //    new ChatListItem()
    //    {
    //        ContactName = "Steve",
    //        ContactPhotoUri = new Uri("/Assets/Images/7.png", UriKind.RelativeOrAbsolute),
    //        LastMessage = "Yes, we had fun.",
    //        LastMessageTime = "Tue, 08:10 AM"
    //    },
    //    new ChatListItem()
    //    {
    //        ContactName = "John",
    //        ContactPhotoUri = new Uri("/Assets/Images/8.jpg", UriKind.RelativeOrAbsolute),
    //        LastMessage = "What about you?",
    //        LastMessageTime = "Tue, 01:00 PM"
    //    },
    //];

    #endregion Logics

    #region Commands

    private IRelayCommand _archiveChatCommand;
    private IRelayCommand _getSelectedChatCommand;

    // To Pin Chat on Pin Button Click
    private IRelayCommand _pinChatCommand;

    private IRelayCommand _unArchiveChatCommand;

    // To get the ContactName of selected chat so that we can open corresponding conversation
    // To Pin Chat on Pin Button Click
    private IRelayCommand _unPinChatCommand;

    public IRelayCommand ArchiveChatCommand => _archiveChatCommand ??= new RelayCommand<ChatListItem>(data =>
    {
        if (!ArchivedChats.Contains(data))
        {
            // Remember, Chat will be removed from Pinned List when Archive.. and Vice Versa..

            // Add Chat in Archive List
            ArchivedChats.Add(data);
            data.IsArchived = true;
            data.IsPinned = false;

            // Remove Chat from Pinned & Unpinned Chat List
            Chats.Remove(data);
            FilteredChats.Remove(data);
            PinnedChats.Remove(data);
            FilteredPinnedChats.Remove(data);
        }
    });

    public IRelayCommand GetSelectedChatCommand => _getSelectedChatCommand ??= new RelayCommand<ChatListItem>(data =>
    {
        if (data == null)
            return;

        //Getting ContactName from selected chat
        ContactName = data.ContactName;

        //Getting ContactPhotoUri from selected chat
        ContactPhotoUri = data.ContactPhotoUri;

        LoadChatConversation(data);
    });

    public IRelayCommand PinChatCommand => _pinChatCommand ??= new RelayCommand<ChatListItem>(data =>
    {
        if (data == null)
            return;

        if (!FilteredPinnedChats.Contains(data))
        {
            // Add selected chat to pin chat
            PinnedChats.Add(data);
            FilteredPinnedChats.Add(data);
            data.IsPinned = true;

            // Store position of chat before pinning so that when we unpin or un archive we get it on same original position...
            _chatPosition = Chats.IndexOf(data);

            // Remove selected chat from all chats / unpinned chats
            Chats.Remove(data);
            FilteredChats.Remove(data);

            // Fixed
            // Remember, Chat will be removed from Pinned List when Archive.. and Vice Versa..
            if (ArchivedChats != null)
            {
                if (ArchivedChats.Contains(data))
                {
                    ArchivedChats.Remove(data);
                    data.IsArchived = false;
                }
            }
        }
    });

    public IRelayCommand UnArchiveChatCommand => _unArchiveChatCommand ??= new RelayCommand<ChatListItem>(data =>
    {
        if (!FilteredChats.Contains(data) && !Chats.Contains(data))
        {
            Chats.Add(data);
            FilteredChats.Add(data);
        }
        ArchivedChats.Remove(data);
        data.IsArchived = false;
        data.IsPinned = false;
    });

    public IRelayCommand UnPinChatCommand => _unPinChatCommand ??= new RelayCommand<ChatListItem>(data =>
    {
        if (data == null)
            return;

        if (!FilteredChats.Contains(data))
        {
            // Add selected chat to Normal Unpinned chat list
            Chats.Add(data);
            FilteredChats.Add(data);

            // Restore position of chat before pinning so that when we unpin or un archive we get it on same original position...
            Chats.Move(Chats.Count - 1, _chatPosition);
            FilteredChats.Move(Chats.Count - 1, _chatPosition);

            // Remove selected pinned chats list
            PinnedChats.Remove(data);
            FilteredPinnedChats.Remove(data);
            data.IsPinned = false;
        }
    });

    #endregion Commands

    #endregion Chat List

    #region Conversation

    #region Fields

    private ObservableCollection<ChatConversation> _conversations;
    private ObservableCollection<ChatConversation> _filteredConversations;

    #endregion Fields

    #region Properties

    private bool _isSearchConversationBoxOpen;

    private string _lastSearchConversationText;

    private string _searchConversationText;

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

    #region Ctors

    private void OnChatConversationEvent(ChatConversation conversation)
    {
        FilteredConversations.Add(conversation);
        Conversations.Add(conversation);
    }

    private async void OnChatListDataEvent(ChatListDataEventArgs chatEvent)
    {
        await LoadChatConversation(chatEvent.Data);
    }

    #endregion Ctors

    #region Commands

    private IRelayCommand _clearConversationSearchCommand;
    private IRelayCommand _relayCommand;
    private IRelayCommand _searchConversationCommand;
    public IRelayCommand ClearConversationSearchCommand => _clearConversationSearchCommand ??= new RelayCommand(ClearConversationSearchBox);

    public IRelayCommand RelayCommand => _relayCommand ??= new RelayCommand<ChatConversation>(data =>
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

    private async Task LoadChatConversation(ChatListItem chat)
    {
        Conversations ??= [];

        Conversations.Clear();
        FilteredConversations.Clear();

        try
        {
            var conversations = await _chatService.GetConversationsByContactNameAsync(chat.ContactName);

            if (conversations.Count() > 0)
            {
                var conversation = conversations.LastOrDefault();
                chat.LastMessage = !string.IsNullOrEmpty(conversation.ReceivedMessage) ? conversation.ReceivedMessage : conversation.SentMessage;
            }

            Conversations = new ObservableCollection<ChatConversation>(conversations);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading chat conversations: {ex.Message}");
        }

        // Reset reply message text when the new chat is fetched.
        _eventAggregator.Publish(new ReplyMessageEventArgs
        {
            MessageToReplyText = string.Empty
        });
    }

    private bool MatchesSearch(ChatConversation chat, string searchText)
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
            FilteredConversations = new ObservableCollection<ChatConversation>(Conversations ?? Enumerable.Empty<ChatConversation>());

            // Update Last serach Text
            _lastSearchConversationText = SearchConversationText;
            return;
        }

        FilteredConversations = new ObservableCollection<ChatConversation>(
            Conversations.Where(chat => MatchesSearch(chat, SearchConversationText))
        );

        // Update Last serach Text
        _lastSearchConversationText = SearchConversationText;
    }

    #endregion Logics

    #endregion Conversation

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
                    ContactPhotoUri = new Uri("/Assets/Images/1.png", UriKind.RelativeOrAbsolute),
                    StatusImageUri = new Uri("/Assets/Images/8.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "Steve",
                    ContactPhotoUri = new Uri("/Assets/Images/2.jpg", UriKind.RelativeOrAbsolute),
                    StatusImageUri = new Uri("/Assets/Images/7.png", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "Will",
                    ContactPhotoUri = new Uri("/Assets/Images/3.jpg", UriKind.RelativeOrAbsolute),
                    StatusImageUri = new Uri("/Assets/Images/6.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "John",
                    ContactPhotoUri = new Uri("/Assets/Images/4.jpg", UriKind.RelativeOrAbsolute),
                    StatusImageUri = new Uri("/Assets/Images/5.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
            ];
    }

    #endregion Logics

    #endregion Status Thumbs
}