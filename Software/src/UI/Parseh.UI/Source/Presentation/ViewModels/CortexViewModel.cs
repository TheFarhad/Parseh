namespace Parseh.UI.ViewModels;

internal sealed class CortexViewModel : VM
{
    public PageMode Mode { get => Get(); private set => Set(value); }
    public ContentPage Page { get => Get(); private set => Set(value); }
    public bool IsOpenSettingMenu { get => Get(); set => Set(value); }
    public bool ToggleAttachmentMenu { get => Get(); set => Set(value); }
    public SettingViewModel SettingModel { get => Get(); private set => Set(value); }

    public string Test { get => Get(); set => Set(value); }

    public CortexViewModel() => Init();

    public void ToPage(PageMode mode)
    {
        if (Mode != mode) Pager(mode);
    }

    #region Private Functionality

    void Init()
    {
        Test = "Write your message";
        Mode = PageMode.Chat;
        Pager(Mode);
        IsOpenSettingMenu = false;
        ToggleAttachmentMenu = false;
        SettingModel = NetIoC.Default.Get<SettingViewModel>()!;
    }

    void Pager(PageMode mode)
    {
        Page = mode switch
        {
            PageMode.Signin => new Signin(),
            PageMode.Signup => new Signup(),
            PageMode.Chat => new Chat(),
            _ => throw new NotImplementedException("A page for this mode has not been implemented."),
        };
        Mode = mode;
    }

    #endregion
}

internal sealed class Cortex
{
    public static readonly Cortex Default = new();

    Cortex()
    {
        var ioc = NetIoC.Default;
        Model = ioc.GetRequired<CortexViewModel>();
    }

    public CortexViewModel Model { get; } = new();
}