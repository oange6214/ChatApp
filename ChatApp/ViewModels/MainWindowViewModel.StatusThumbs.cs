using ChatApp.Core.Models;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModels;

public partial class MainWindowViewModel
{
    #region Fields

    private ObservableCollection<StatusDataModelDto> _statuses;

    #endregion Fields

    #region Properties

    public ObservableCollection<StatusDataModelDto> StatusThumbsCollection
    {
        get => _statuses;
        set => SetProperty(ref _statuses, value);
    }

    #endregion Properties

    #region Logics

    private void LoadStatusThumbs()
    {
        StatusThumbsCollection =
            [
                new StatusDataModelDto
                {
                    IsMeAddStatus = true,
                },
                new StatusDataModelDto
                {
                    ContactName = "Mike",
                    ContactPhotoUri = new Uri("/Assets/Images/1.png", UriKind.RelativeOrAbsolute),
                    StatusImageUri = new Uri("/Assets/Images/8.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModelDto
                {
                    ContactName = "Steve",
                    ContactPhotoUri = new Uri("/Assets/Images/2.jpg", UriKind.RelativeOrAbsolute),
                    StatusImageUri = new Uri("/Assets/Images/7.png", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModelDto
                {
                    ContactName = "Will",
                    ContactPhotoUri = new Uri("/Assets/Images/3.jpg", UriKind.RelativeOrAbsolute),
                    StatusImageUri = new Uri("/Assets/Images/6.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
                new StatusDataModelDto
                {
                    ContactName = "John",
                    ContactPhotoUri = new Uri("/Assets/Images/4.jpg", UriKind.RelativeOrAbsolute),
                    StatusImageUri = new Uri("/Assets/Images/5.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },
            ];
    }

    #endregion Logics
}