namespace ChatApp.EventArgs;

public class DataEventArgs : System.EventArgs
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public object Data { get; set; }
}