namespace Framework;

using System.Windows.Input;

public sealed class Command : ICommand
{
    public event EventHandler? CanExecuteChanged = (sender, eventArgs) => { };
    private readonly Action _execute;
    private readonly Func<bool> _canexecute = () => true;

    public Command(Action execute, Func<bool> canexecute = default!)
    {
        _execute = execute;
        if (canexecute is { }) _canexecute = canexecute;
    }

    public bool CanExecute(object? parameter) => _canexecute.Invoke();
    public void Execute(object? parameter) => _execute?.Invoke();
}

public sealed class Command<Input> : ICommand
{
    public event EventHandler? CanExecuteChanged = (sender, eventArgs) => { };
    private readonly Action<Input> _execute;
    private readonly Func<Input, bool> _canexecute = _ => true;

    public Command(Action<Input> execute, Func<Input, bool> canexecute = default!)
    {
        _execute = execute;
        if (canexecute is { }) _canexecute = canexecute;
    }

    public bool CanExecute(object? parameter) => _canexecute.Invoke((Input)parameter!);
    public void Execute(object? parameter) => _execute?.Invoke((Input)parameter!);
}

