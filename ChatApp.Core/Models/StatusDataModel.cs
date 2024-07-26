namespace ChatApp.Core.Models;

public class StatusDataModel
{
    public string ContactName { get; set; } = string.Empty;

    public Uri? ContactPhotoUri { get; set; }

    //If we want to add our status
    public bool IsMeAddStatus { get; set; }

    public Uri? StatusImageUri { get; set; }

    /// <summary>
    /// We will be covering in one of our upcoming videos
    /// TODO: Status LastMessage
    /// </summary>
    //public string StatusMessage { get; set; }
}