namespace Parseh.UI.ViewModels;

using Views;


public sealed class NotifierViewModel : BaseLayoutViewModel
{
    public DialogMessageType Type { get => Get(); set => Set(value); }

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
        Type = DialogMessageType.Information;
    }
}



