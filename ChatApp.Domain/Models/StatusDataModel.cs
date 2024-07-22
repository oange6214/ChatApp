namespace ChatApp.Domain.Models;

public class StatusDataModel
{
    public string ContactName { get; set; }
    public Uri ContactPhoto { get; set; }
    public Uri StatusImage { get; set; }

    //If we want to add our status
    public bool IsMeAddStatus { get; set; }

    /// <summary>
    /// We will be covering in one of our upcoming videos
    /// TODO: Status Message
    /// </summary>
    //public string StatusMessage { get; set; }
}