using ChatApp.Models;
using ChatApp.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using Toolkit.Wpf.Mvvm.ComponentModel;
using Toolkit.Wpf.Mvvm.Input;
using Toolkit.Wpf.Mvvm.Messaging;
using Toolkit.Wpf.Mvvm.Messaging.Interfaces;

namespace ChatApp.ViewModels;

public class MainWindowViewModel : ObservableObject, IMainWindowViewModel
{
    #region Fields

    private IEventAggregator _eventAggregator;
    private string _contactName;
    private Uri _contactPhoto;
    private string _lastSearchText;
    private string _lastSeen;
    private string _searchText;
    private WindowState _windowState;
    private string _messageText;

    #endregion Fields

    #region Properties

    public string MessageText
    {
        get => _messageText;
        set => SetProperty(ref _messageText, value);
    }

    public IChatListViewModel ChatListVM { get; }
    public IConversationViewModel ConversationVM { get; }

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

    public IStatusThumbsViewModel StatusThumbsVM { get; }

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

        _eventAggregator.Subscribe<ChatSelectedEvent>(OnChatSelected);
    }

    #endregion Ctors

    #region Event Aggregator Methods

    private void OnChatSelected(ChatSelectedEvent evt)
    {
        ContactName = evt.ContactName;
        ContactPhoto = evt.ContactPhoto;
    }

    #endregion Event Aggregator Methods

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
        if (string.IsNullOrEmpty(SearchText) || ChatListVM.Chats == null || ChatListVM.Chats.Count <= 0)
        {
            ChatListVM.FilteredChats = new ObservableCollection<ChatListData>(ChatListVM.Chats ?? Enumerable.Empty<ChatListData>());
            ChatListVM.FilteredPinnedChats = new ObservableCollection<ChatListData>(ChatListVM.PinnedChats ?? Enumerable.Empty<ChatListData>());

            // Update Last serach Text
            LastSearchText = SearchText;

            return;
        }

        // Now, to find the all chats that contain the text in our search box

        // if that chat is in Normal Unpinned Chat list find there...

        ChatListVM.FilteredChats = new ObservableCollection<ChatListData>(
            ChatListVM.Chats.Where(
                chat => chat.ContactName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) // if ContactName Contains SearchText then add it in filtered chat list
                ||
                chat.Message != null & chat.Message.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) // if Message Contains SearchText then add it in filtered chat list
                )
            );

        // else if not found in Normal Unpinned Chat list, find in pinned chats list
        ChatListVM.FilteredPinnedChats = new ObservableCollection<ChatListData>(
            ChatListVM.PinnedChats.Where(
                pinnedChat => pinnedChat.ContactName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) // if ContactName Contains SearchText then add it in filtered chat list
                ||
                pinnedChat.Message != null & pinnedChat.Message.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) // if Message Contains SearchText then add it in filtered chat list
                )
            );

        // Update Last serach Text
        LastSearchText = SearchText;
    }

    #endregion Logics
}