using ChatApp.Domain.Models;
using ChatApp.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using Toolkit.Wpf.Mvvm.ComponentModel;

namespace ChatApp.ViewModels;

public class StatusThumbsViewModel : ObservableObject, IStatusThumbsViewModel
{
    #region Fields

    private ObservableCollection<StatusDataModel> _statuses;

    #endregion Fields

    #region Properties

    public ObservableCollection<StatusDataModel> StatusThumbsCollection
    {
        get => _statuses;
        set => SetProperty(ref _statuses, value);
    }

    #endregion Properties

    #region Ctors

    public StatusThumbsViewModel()
    {
        LoadStatusThumbs();
    }

    #endregion Ctors

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
                    StatusImage = new Uri("/Assets/Images/8.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "Steve",
                    ContactPhoto = new Uri("/Assets/Images/2.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/7.png", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "Will",
                    ContactPhoto = new Uri("/Assets/Images/3.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/6.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModel
                {
                    ContactName = "John",
                    ContactPhoto = new Uri("/Assets/Images/4.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Images/5.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
            ];
    }

    #endregion Logics
}