using System.Windows.Input;

namespace Toolkit.Wpf.Mvvm.Input;

public interface IRelayCommand : ICommand
{
    void NotifyCanExecuteChanged();
}