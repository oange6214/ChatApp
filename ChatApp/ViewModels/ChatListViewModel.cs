﻿using ChatApp.Models;
using ChatApp.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using Toolkit.Wpf.Mvvm.ComponentModel;
using Toolkit.Wpf.Mvvm.Input;
using Toolkit.Wpf.Mvvm.Messaging;
using Toolkit.Wpf.Mvvm.Messaging.Interfaces;

namespace ChatApp.ViewModels;

public class ChatListViewModel : ObservableObject, IChatListViewModel
{
    #region Fields

    private IEventAggregator _eventAggregator;
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

    #region Ctors

    //public ChatListViewModel()
    //{ }

    public ChatListViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
        LoadChats();
    }

    #endregion Ctors

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

    #region Commands

    private IRelayCommand _getSelectedChatCommand;

    public IRelayCommand GetSelectedChatCommand => _getSelectedChatCommand ??= new RelayCommand<ChatListData>(data =>
    {
        if (data == null)
            return;

        _eventAggregator.Publish(new ChatSelectedEvent
        {
            //Getting ContactName from selected chat
            ContactName = data.ContactName,

            //Getting ContactPhoto from selected chat
            ContactPhoto = data.ContactPhoto
        });
    });

    #region Commands

    // To get the ContactName of selected chat so that we can open corresponding conversation

    // To Pin Chat on Pin Button Click
    private IRelayCommand _pinChatCommand;

    // To Pin Chat on Pin Button Click
    private IRelayCommand _unPinChatCommand;

    private IRelayCommand _archiveChatCommand;

    public IRelayCommand ArchiveChatCommand => _archiveChatCommand ??= new RelayCommand<ChatListData>(data =>
    {
    });

    public IRelayCommand PinChatCommand => _pinChatCommand ??= new RelayCommand<ChatListData>(data =>
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

    public IRelayCommand UnPinChatCommand => _unPinChatCommand ??= new RelayCommand<ChatListData>(data =>
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

    #endregion Commands
}