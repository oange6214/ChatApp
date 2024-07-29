using System.Windows.Input;

namespace Toolkit.Mvvm.Wpf.Input;

public interface IRelayCommand : ICommand
{
    void NotifyCanExecuteChanged();
}