using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChatApp.Core.Models;

public class ChatListItem : INotifyPropertyChanged
{
    private string _lastMessage = string.Empty;
    private string _lastMessageTime = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string ContactName { get; set; } = string.Empty;
    public byte[]? ContactPhotoUri { get; set; }
    public bool IsArchived { get; set; }
    public bool IsPinned { get; set; }
    public bool IsSelected { get; set; }

    public string LastMessage
    {
        get => _lastMessage;
        set
        {
            _lastMessage = value;
            OnPropertyChanged();
        }
    }

    public string LastMessageTime
    {
        get => _lastMessageTime;
        set
        {
            _lastMessageTime = value;
            OnPropertyChanged();
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}