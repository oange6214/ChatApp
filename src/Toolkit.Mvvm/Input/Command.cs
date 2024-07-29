using System.ComponentModel;
using System.Windows.Input;

namespace Toolkit.Mvvm.Input;

/// <summary>
/// Class Command. Implements the <see cref="System.Windows.Input.ICommand"/> Implements the
/// <see cref="System.IDisposable"/>
/// </summary>
/// <typeparam name="T">The type of command parameter.</typeparam>
/// <seealso cref="System.Windows.Input.ICommand"/>
/// <seealso cref="System.IDisposable"/>
public class Command<T> : ICommand, IDisposable
{
    /// <summary>
    /// The can execute
    /// </summary>
    private readonly Predicate<T> canExecute;

    /// <summary>
    /// The execute
    /// </summary>
    private readonly Action<T> execute;

    /// <summary>
    /// The is disposed
    /// </summary>
    private bool isDisposed;

    /// <summary>
    /// The object to listen
    /// </summary>
    private INotifyPropertyChanged objectToListen;

    /// <summary>
    /// Initializes a new instance of the <see cref="Command{T}"/> class.
    /// </summary>
    /// <param name="execute">The execute method.</param>
    public Command(Action<T> execute)
      : this(execute, (Predicate<T>)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Command{T}"/> class.
    /// </summary>
    /// <param name="execute">The execute method.</param>
    /// <param name="canExecute">The can execute method.</param>
    public Command(Action<T> execute, Predicate<T> canExecute) : this(execute, canExecute, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Command{T}"/> class.
    /// </summary>
    /// <param name="execute">The execute method.</param>
    /// <param name="canExecute">The can execute method.</param>
    /// <param name="objectToListen">The object to listen.</param>
    /// <param name="propertiesToListen">
    /// The properties to listen. Null if all the properties are to be listened.
    /// </param>
    public Command(Action<T> execute, Predicate<T> canExecute, INotifyPropertyChanged objectToListen, IEnumerable<string> propertiesToListen = null)
    {
        execute = execute;
        canExecute = canExecute;
        objectToListen = objectToListen;

        if (objectToListen != null)
        {
            objectToListen.PropertyChanged += ObjectPropertyChanged;
            PropertiesToListen = propertiesToListen;
        }
    }

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add
        {
            CanExecuteChangeValue += value;
        }

        remove
        {
            CanExecuteChangeValue -= value;
        }
    }

    /// <summary>
    /// Occurs when [can execute change value].
    /// </summary>
    private event EventHandler CanExecuteChangeValue;

    /// <summary>
    /// Gets the properties to listen.
    /// </summary>
    /// <value>The properties to listen.</value>
    public IEnumerable<string> PropertiesToListen { get; private set; }

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed, this object
    /// can be set to <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if this command can be executed; otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanExecute(object parameter) => canExecute?.Invoke((T)parameter) ?? true;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting
    /// unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed, this object
    /// can be set to <see langword="null"/>.
    /// </param>
    public void Execute(object parameter) => execute((T)parameter);

    /// <summary>
    /// Raises the can execute changed.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChangeValue?.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
    /// only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing)
        {
            if (objectToListen != null)
            {
                objectToListen.PropertyChanged -= ObjectPropertyChanged;
            }

            objectToListen = null;
        }

        isDisposed = true;
    }

    /// <summary>
    /// Listens to any property changed of the object that is passed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    /// The <see cref="PropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void ObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (PropertiesToListen == null || ((PropertiesToListen?.Any() ?? false) && PropertiesToListen.Contains(e.PropertyName)))
        {
            // Raise on all the property change
            RaiseCanExecuteChanged();
        }
    }
}