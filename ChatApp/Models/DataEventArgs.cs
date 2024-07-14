namespace ChatApp.Models;

public class DataEventArgs : EventArgs
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public object Data { get; set; }
}