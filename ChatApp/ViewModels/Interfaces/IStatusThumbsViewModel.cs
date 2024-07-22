using ChatApp.Domain.Models;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModels.Interfaces;

public interface IStatusThumbsViewModel
{
    ObservableCollection<StatusDataModel> StatusThumbsCollection { get; set; }
}