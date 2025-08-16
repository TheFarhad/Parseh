namespace Framework;

using System.Windows.Input;

public interface IRelayCommand : ICommand
{
    void Notify();
}

public sealed class Command : IRelayCommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canexecute = () => true;
    public event EventHandler? CanExecuteChanged = (sender, eventArgs) => { };

    public Command(Action execute, Func<bool> canexecute = default!)
    {
        _execute = execute;
        if (canexecute is not null) _canexecute = canexecute;
    }

    public void Notify() => CanExecuteChanged!.Invoke(this, EventArgs.Empty);
    public bool CanExecute(object? parameter) => _canexecute!.Invoke();
    public void Execute(object? parameter) => _execute?.Invoke();
}

public sealed class Command<TInput> : IRelayCommand
{
    private readonly Action<TInput> _execute;
    private readonly Func<TInput, bool> _canexecute = _ => true;
    public event EventHandler? CanExecuteChanged = (sender, eventArgs) => { };

    public Command(Action<TInput> execute, Func<TInput, bool> canexecute = default!)
    {
        _execute = execute;
        if (canexecute is { }) _canexecute = canexecute;
    }

    public void Notify() => CanExecuteChanged!.Invoke(this, EventArgs.Empty);
    public bool CanExecute(object? parameter) => _canexecute.Invoke(parameter!.As<TInput>());
    public void Execute(object? parameter) => _execute?.Invoke(parameter!.As<TInput>());
}

