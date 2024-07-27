using ChatApp.Core.Interfaces;
using ChatApp.Core.Models;
using System.Collections.ObjectModel;
using Toolkit.Mvvm.Input;

namespace ChatApp.ViewModels;

public partial class MainWindowViewModel
{
    #region Chat List

    #region Fields

    private ObservableCollection<ChatListItemDto> _archivedChats = [];
    private ObservableCollection<ChatListItemDto> _chats = [];
    private IChatService _chatService;
    private ObservableCollection<ChatListItemDto> _filteredChats = [];
    private ObservableCollection<ChatListItemDto> _filteredPinnedChats = [];
    private ObservableCollection<ChatListItemDto> _pinnedChats = [];

    #endregion Fields

    #region Properties

    private int _chatPosition;

    public ObservableCollection<ChatListItemDto> ArchivedChats
    {
        get => _archivedChats;
        set => SetProperty(ref _archivedChats, value);
    }

    public ObservableCollection<ChatListItemDto> Chats
    {
        get => _chats;
        set
        {
            if (SetProperty(ref _chats, value))
            {
                // Updating filtered chats to match
                FilteredChats = new ObservableCollection<ChatListItemDto>(_chats);
            }
        }
    }

    // Filtering Chats & Pinned Chats
    public ObservableCollection<ChatListItemDto> FilteredChats
    {
        get => _filteredChats;
        set => SetProperty(ref _filteredChats, value);
    }

    public ObservableCollection<ChatListItemDto> FilteredPinnedChats
    {
        get => _filteredPinnedChats;
        set => SetProperty(ref _filteredPinnedChats, value);
    }

    public ObservableCollection<ChatListItemDto> PinnedChats
    {
        get => _pinnedChats;
        set
        {
            if (SetProperty(ref _pinnedChats, value))
            {
                // Updating filtered chats to match
                FilteredPinnedChats = new ObservableCollection<ChatListItemDto>(_pinnedChats);
            }
        }
    }

    #endregion Properties

    #region Logics

    private async void LoadChats()
    {
        // Loading data from Database
        Chats ??= [];

        // Transfer data
        Chats = await _chatService.GetChatListAsync();
    }

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

    public IRelayCommand ArchiveChatCommand => _archiveChatCommand ??= new RelayCommand<ChatListItemDto>(data =>
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

    public IRelayCommand GetSelectedChatCommand => _getSelectedChatCommand ??= new RelayCommand<ChatListItemDto>(data =>
    {
        if (data == null)
            return;

        //Getting ContactName from selected chat
        ContactName = data.ContactName;

        //Getting ContactPhotoUri from selected chat
        ContactPhotoUri = data.ContactPhotoUri;

        LoadChatConversation(data);
    });

    public IRelayCommand PinChatCommand => _pinChatCommand ??= new RelayCommand<ChatListItemDto>(data =>
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

    public IRelayCommand UnArchiveChatCommand => _unArchiveChatCommand ??= new RelayCommand<ChatListItemDto>(data =>
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

    public IRelayCommand UnPinChatCommand => _unPinChatCommand ??= new RelayCommand<ChatListItemDto>(data =>
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
}