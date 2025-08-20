namespace Parseh.UI.ViewModels;

public abstract class DialogMessageViewModel : ViewModel
{
    public string Title { get => Get(); set => Set(value); }
    public string Message { get => Get(); set => Set(value); }
    public DialogMessageType Type { get => Get(); set => Set(value); }

    public DialogMessageViewModel() => Init();

    void Init()
    {
        Type = DialogMessageType.Information;
    }
}

public enum DialogMessageType
{
    Information = 0,
    Warning = 1,
    Error = 2,
}




