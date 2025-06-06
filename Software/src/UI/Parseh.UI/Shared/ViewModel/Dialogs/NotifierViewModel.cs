namespace Parseh.UI.ViewModels;

using Views;

public sealed class NotifierViewModel : BaseLayoutViewModel
{
    public MessageType Type { get => Get(); set => Set(value); }

    public NotifierViewModel(Notifier layout) : base(layout)
        => Init();

    void Init()
    {
        InitProperties();
    }

    void InitProperties()
    {
        MinWidth = 250;
        MinHeight = 70;
        Type = MessageType.Information;
    }
}

public abstract class DialogMessageViewModel : VM
{
    public string Title { get => Get(); set => Set(value); }
    public string Message { get => Get(); set => Set(value); }
    public MessageType Type { get => Get(); set => Set(value); }

    public DialogMessageViewModel() => Init();

    void Init()
    {
        Type = MessageType.Information;
    }
}

public enum MessageType
{
    Information = 0,
    Warning = 1,
    Error = 2,
}

