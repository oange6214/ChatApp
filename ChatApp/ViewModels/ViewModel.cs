using ChatApp.Models;
using System.Collections.ObjectModel;
using Toolkit.Wpf.Mvvm.ComponentModel;

namespace ChatApp.ViewModels;

public class ViewModel : ObservableObject
{
    #region Status Thumbs

    #region Fields

    private ObservableCollection<StatusDataModel> _statuses;

    #endregion

    #region Properties

    public ObservableCollection<StatusDataModel> StatusThumbsCollection
    {
        get => _statuses;
        set => SetProperty(ref _statuses, value);
    }

    #endregion

    #region Logics

    private void LoadStatusThumbs()
    {
        StatusThumbsCollection =
        [
            new StatusDataModel
            {
                IsMeAddStatus = true,
            },
            new StatusDataModel
            {
                ContactName = "Mike",
                ContactPhoto = new Uri("/Assets/Images/1.png", UriKind.RelativeOrAbsolute),
                StatusImage = new Uri("/Assets/Images/5.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus = false
            },
            new StatusDataModel
            {
                ContactName = "Steve",
                ContactPhoto = new Uri("/Assets/Images/2.jpg", UriKind.RelativeOrAbsolute),
                StatusImage = new Uri("/Assets/Images/8.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus = false
            },
            new StatusDataModel
            {
                ContactName = "Will",
                ContactPhoto = new Uri("/Assets/Images/3.jpg", UriKind.RelativeOrAbsolute),
                StatusImage = new Uri("/Assets/Images/5.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus = false
            },
            new StatusDataModel
            {
                ContactName = "John",
                ContactPhoto = new Uri("/Assets/Images/4.jpg", UriKind.RelativeOrAbsolute),
                StatusImage = new Uri("/Assets/Images/3.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus = false
            },
        ];
    }

    #endregion

    #endregion

    public ViewModel()
    {
        LoadStatusThumbs();
    }
}