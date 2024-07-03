using System.Windows;
using Toolkit.Wpf.Mvvm.ComponentModel;
using Toolkit.Wpf.Mvvm.Input;

namespace ChatApp.ViewModels;

internal class MainViewModel : ObservableObject
{
    private WindowState _windowState;

    public IRelayCommand CloseCommand => new RelayCommand(Close);

    public IRelayCommand MaximizeCommand => new RelayCommand(Maximize);

    public IRelayCommand MinimizeCommand => new RelayCommand(Minimize);

    public WindowState WindowState
    {
        get => _windowState;
        set => SetProperty(ref _windowState, value);
    }

    private void Close()
    {
        Application.Current.Shutdown();
    }

    private void Maximize()
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
        }
        else
        {
            WindowState = WindowState.Normal;
        }
    }

    private void Minimize()
    {
        WindowState = WindowState.Minimized;
    }
}