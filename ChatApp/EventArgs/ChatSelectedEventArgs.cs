namespace ChatApp.EventArgs;

public class ChatSelectedEventArgs
{
    public string ContactName { get; set; }
    public byte[]? ContactPhotoUri { get; set; }
}