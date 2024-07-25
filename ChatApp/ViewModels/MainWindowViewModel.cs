using ChatApp.Domain.Models;
using ChatApp.EventArgs;
using ChatApp.Models;
using ChatApp.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Media;
using Toolkit.Wpf.Mvvm.ComponentModel;
using Toolkit.Wpf.Mvvm.Input;
using Toolkit.Wpf.Mvvm.Messaging.Interfaces;

namespace ChatApp.ViewModels;

public class MainWindowViewModel : ObservableObject, IMainWindowViewModel
{
    // Initializing resource dictionary file
    private readonly ResourceDictionary dictionary = Application.LoadComponent(new Uri("/ChatApp;component/Assets/Images/icons.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;

    #region Fields

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

    public IChatListViewModel ChatListVM { get; }

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

    public IConversationViewModel ConversationVM { get; }

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

    public IStatusThumbsViewModel StatusThumbsVM { get; }

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

    #region Ctors

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(
        IEventAggregator eventAggregator,
        IChatListViewModel chatListVM,
        IConversationViewModel conversationVM,
        IStatusThumbsViewModel statusThumbsVM)
    {
        _eventAggregator = eventAggregator;
        StatusThumbsVM = statusThumbsVM;
        ChatListVM = chatListVM;
        ConversationVM = conversationVM;

        _eventAggregator.Subscribe<ChatSelectedEventArgs>(OnChatSelectedEvent);
        _eventAggregator.Subscribe<ReplyMessageEventArgs>(OnReplyMessageEvent);
    }

    #endregion Ctors

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

    public void OpenConversationSearchBox() => ConversationVM.IsSearchConversationBoxOpen = true;

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
            _eventAggregator.Publish(conversation);

            UpdateChatAndMoveUp(ChatListVM.Chats, conversation);
            UpdateChatAndMoveUp(ChatListVM.PinnedChats, conversation);
            UpdateChatAndMoveUp(ChatListVM.FilteredChats, conversation);
            UpdateChatAndMoveUp(ChatListVM.FilteredPinnedChats, conversation);
            UpdateChatAndMoveUp(ChatListVM.ArchivedChats, conversation);

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
        if (string.IsNullOrWhiteSpace(SearchText) || ChatListVM.Chats == null || ChatListVM.Chats.Count <= 0)
        {
            ChatListVM.FilteredChats = new ObservableCollection<ChatListItem>(ChatListVM.Chats ?? Enumerable.Empty<ChatListItem>());
            ChatListVM.FilteredPinnedChats = new ObservableCollection<ChatListItem>(ChatListVM.PinnedChats ?? Enumerable.Empty<ChatListItem>());

            // Update Last serach Text
            _lastSearchText = SearchText;
            return;
        }

        // Now, to find the all chats that contain the text in our search box, if that chat is in Normal Unpinned Chat list find there...

        Func<ChatListItem, bool> searchPredicate = chat =>
            chat.ContactName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)  // if ContactName Contains SearchText then add it in filtered chat list
            || (chat.LastMessage != null && chat.LastMessage.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)); // if LastMessage Contains SearchText then add it in filtered chat list

        ChatListVM.FilteredChats = new ObservableCollection<ChatListItem>(ChatListVM.Chats.Where(searchPredicate));

        // else if not found in Normal Unpinned Chat list, find in pinned chats list
        ChatListVM.FilteredPinnedChats = new ObservableCollection<ChatListItem>(ChatListVM.PinnedChats.Where(searchPredicate));

        // Update Last serach Text
        _lastSearchText = SearchText;
    }

    #endregion Logics
}