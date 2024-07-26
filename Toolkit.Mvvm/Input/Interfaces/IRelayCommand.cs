using System.Windows.Input;

namespace Toolkit.Mvvm.Input;

public interface IRelayCommand : ICommand
{
    void NotifyCanExecuteChanged();
}