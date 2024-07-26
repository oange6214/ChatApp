using ChatApp.Core.Interfaces;
using ChatApp.Core.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using Toolkit.Mvvm.Input;

namespace ChatApp.ViewModels;

public partial class MainWindowViewModel
{
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

        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Workspaces\Devolops\WPFs\ChatApp\ChatApp.Infrastructure\Database\Database1.mdf;Integrated Security=True";
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
}