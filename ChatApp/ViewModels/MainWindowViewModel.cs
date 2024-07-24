using ChatApp.Domain.Models;
using ChatApp.EventArgs;
using ChatApp.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using Toolkit.Wpf.Mvvm.ComponentModel;
using Toolkit.Wpf.Mvvm.Input;
using Toolkit.Wpf.Mvvm.Messaging.Interfaces;

namespace ChatApp.ViewModels;

public class MainWindowViewModel : ObservableObject, IMainWindowViewModel
{
    #region Fields

    private string _contactName;
    private Uri _contactPhoto;
    private IEventAggregator _eventAggregator;
    private bool _isSearchBoxOpen;
    private string _lastSearchText;
    private string _lastSeen;
    private string _messageText;
    private string _searchText;
    private WindowState _windowState;

    #endregion Fields

    #region Properties

    public IChatListViewModel ChatListVM { get; }

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
        ContactPhoto = evt.ContactPhoto;
    }

    private void OnReplyMessageEvent(ReplyMessageEventArgs args)
    {
        FocusMessageBox = args.FocusMessageBox;
        MessageToReplyText = args.MessageToReplyText;
        IsThisAReplyMessage = args.IsThisAReplyMessage;
    }

    #endregion Event Aggregator Methods

    #region Commands

    private IRelayCommand _cancelReplyCommand;
    private IRelayCommand _clearSearchCommand;
    private IRelayCommand _closeCommand;
    private IRelayCommand _maximizeCommand;
    private IRelayCommand _minimizeCommand;
    private IRelayCommand _openConversationSearchCommand;
    private IRelayCommand _openSearchCommand;
    private IRelayCommand _searchCommand;
    private IRelayCommand _sendMessageCommand;
    public IRelayCommand CancelReplyCommand => _cancelReplyCommand ??= new RelayCommand(CancelReply);
    public IRelayCommand ClearSearchCommand => _clearSearchCommand ??= new RelayCommand(ClearSearchBox);
    public IRelayCommand CloseCommand => _closeCommand ??= new RelayCommand(Close);
    public IRelayCommand MaximizeCommand => _maximizeCommand ??= new RelayCommand(Maximize);
    public IRelayCommand MinimizeCommand => _minimizeCommand ??= new RelayCommand(Minimize);
    public IRelayCommand OpenConversationSearchCommand => _openConversationSearchCommand ??= new RelayCommand(OpenConversationSearchBox);
    public IRelayCommand OpenSearchCommand => _openSearchCommand ??= new RelayCommand(OpenSearchBox);
    public IRelayCommand SearchCommand => _searchCommand ??= new RelayCommand(Search);
    public IRelayCommand SendMessageCommand => _sendMessageCommand ??= new RelayCommand(SendMessage);

    #endregion Commands

    #region Logics

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

            // Clear Mesage properties and textbox when message is sent.
            MessageText = string.Empty;
            MessageToReplyText = string.Empty;
            IsThisAReplyMessage = false;

            // TODO: Add Function to Move the chat contact on top when new message is sent or received.
        }
    }

    public void CancelReply()
    {
        IsThisAReplyMessage = false;

        // Reset Reply Message Text
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
            ChatListVM.FilteredChats = new ObservableCollection<ChatListData>(ChatListVM.Chats ?? Enumerable.Empty<ChatListData>());
            ChatListVM.FilteredPinnedChats = new ObservableCollection<ChatListData>(ChatListVM.PinnedChats ?? Enumerable.Empty<ChatListData>());

            // Update Last serach Text
            _lastSearchText = SearchText;
            return;
        }

        // Now, to find the all chats that contain the text in our search box, if that chat is in Normal Unpinned Chat list find there...

        Func<ChatListData, bool> searchPredicate = chat =>
            chat.ContactName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)  // if ContactName Contains SearchText then add it in filtered chat list
            || (chat.Message != null && chat.Message.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)); // if Message Contains SearchText then add it in filtered chat list

        ChatListVM.FilteredChats = new ObservableCollection<ChatListData>(ChatListVM.Chats.Where(searchPredicate));

        // else if not found in Normal Unpinned Chat list, find in pinned chats list
        ChatListVM.FilteredPinnedChats = new ObservableCollection<ChatListData>(ChatListVM.PinnedChats.Where(searchPredicate));

        // Update Last serach Text
        _lastSearchText = SearchText;
    }

    #endregion Logics
}