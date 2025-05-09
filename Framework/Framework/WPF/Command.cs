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
        if (canexecute is { }) _canexecute = canexecute;
    }

    public void Execute(object? parameter) => _execute?.Invoke();
    public bool CanExecute(object? parameter) => _canexecute!.Invoke();
    public void Notify() => CanExecuteChanged!.Invoke(this, EventArgs.Empty);
}

public sealed class Command<Input> : IRelayCommand
{
    private readonly Action<Input> _execute;
    private readonly Func<Input, bool> _canexecute = _ => true;
    public event EventHandler? CanExecuteChanged = (sender, eventArgs) => { };

    public Command(Action<Input> execute, Func<Input, bool> canexecute = default!)
    {
        _execute = execute;
        if (canexecute is { }) _canexecute = canexecute;
    }

    public void Notify() => CanExecuteChanged!.Invoke(this, EventArgs.Empty);
    public void Execute(object? parameter) => _execute?.Invoke(parameter!.As<Input>());
    public bool CanExecute(object? parameter) => _canexecute.Invoke(parameter!.As<Input>());
}

