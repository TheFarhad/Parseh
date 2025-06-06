namespace Parseh.UI.ViewModels;

public sealed class MessageboxViewModel : DialogMessageViewModel
{
    public string OkText { get => Get(); set => Set(value); }

    public MessageboxViewModel() => Init();

    void Init()
    {
        InitProperties();
    }

    void InitProperties()
    {
        OkText = "Ok";
    }
}
