namespace Toolkit.Mvvm.Input;

public interface IRelayCommand<in T> : IRelayCommand
{
    bool CanExecute(T? parameter);

    void Execute(T? parameter);
}